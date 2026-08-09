using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using HarmonyLib;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// La Hoja de Tilo (菩提叶之弱点) — reliquia starter de Siegfried y motor de su identidad.
/// Al iniciar cada combate otorga <see cref="DragonScalesPower"/> (Sangre de Dragón) inicial.
/// Su gancho <see cref="IDragonScalePiercer"/> hace que el PRIMER golpe que te ALCANZA cada
/// turno ignore 1 escama (la espalda expuesta — la debilidad canónica hecha regla); las
/// acumulaciones restantes todavía reducen ese golpe.
/// Cada ataque que realmente alcanza la Vida refunda +5 NP, con tope de 3 procs/turno. Los golpes
/// anulados por Bloqueo o por la armadura no pagan NP.
///
/// Sincronía con DragonScalesPower (orden del pipeline: ModifyHpLostBeforeOsty del power →
/// LoseHp → AfterDamageReceived de esta reliquia, por golpe): el power consulta
/// <see cref="ShouldPierceScales"/> como LECTURA PURA (a prueba de previews de daño); el
/// consumo del cupo 1/turno ocurre aquí, en el camino real del daño.
/// </summary>
public class LindenLeaf : SiegfriedRelic, IDragonScalePiercer, INpLevelStore
{
    // ---- Gacha de dupes / INpLevelStore (audit 2026-07-05, HIGH) ----
    // Sin un INpLevelStore el medidor de NP capeaba en 100 PARA SIEMPRE (NpCharge.Max lee el nivel
    // del store del jugador): la decision central "abrir a 100 vs banquear a 300" era inalcanzable
    // y el escalado NpLevels.Scale de las cartas NP quedaba clavado en x1. Patron calcado de
    // SummonTicket (Mash) / MorganSummonSeal, incluido el gate >=3 que convive con Driftwood.
    public const string DupeOptionId = "SIEGFRIED_DUPE";

    private int _npLevel = 1;
    private int _dupePity;
    private int _startingScales = 2;

    public override bool ShowCounter => true;

    public override int DisplayAmount => NpLevel;

    [SavedProperty]
    public int NpLevel
    {
        get => _npLevel;
        set
        {
            AssertMutable();
            _npLevel = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int DupePity
    {
        get => _dupePity;
        set
        {
            AssertMutable();
            _dupePity = value;
        }
    }

    /// <summary>
    /// Persistent reward for choosing Siegfried's alternative card reward. It is independent of
    /// the NP dupe roll, so even a failed summon makes future combats start more defensively.
    /// </summary>
    [SavedProperty]
    public int StartingScales
    {
        get => _startingScales;
        set
        {
            AssertMutable();
            _startingScales = Math.Clamp(value, BaseStartingScales, MaximumStartingScales);
        }
    }

    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        if (Owner != player) return false;
        if (alternatives.Count >= 3) return false;
        if (!NpLevels.CanLevelUp(Owner)) return false;

        alternatives.Add(new CardRewardAlternative(DupeOptionId, OnDupeRoll, PostAlternateCardRewardAction.EndSelectionAndCompleteReward));
        return true;
    }

    private async Task OnDupeRoll()
    {
        var gainedNpLevel = await NpLevels.TryRollDupeWithConsolation(Owner);
        var gainedScale = StartingScales < MaximumStartingScales;
        if (gainedScale)
        {
            StartingScales++;
        }
        if (gainedNpLevel || gainedScale)
        {
            Flash();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<FafnirHeartblood>();

    private const int BaseStartingScales = 2;
    private const int MaximumStartingScales = 5;
    private const int NpPerHpHit = 5;
    private const int NpProcCapPerTurn = 3;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    // LECTURA PURA: no muta estado (el consumo va en AfterDamageReceived). El primer golpe
    // que alcanza cada turno atraviesa 1 escama; los siguientes chocan contra todas.
    public bool ShouldPierceScales(ValueProp props, Creature? dealer)
    {
        if (IsPierceSuppressed()) return false; // Tarnkappe: la espalda ya no está expuesta
        return FgoCombatState.GetTurn(Owner.Creature, 2) == 0;
    }

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        await PowerCmd.Apply<DragonScalesPower>(new BlockingPlayerChoiceContext(), Owner.Creature, StartingScales, Owner.Creature, null);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature) return;
        if (!props.IsPoweredAttack()) return;
        // Nada llego siquiera al Bloqueo (ataque reducido a 0 antes, p.ej. 1 de dano con Weak):
        // NO es un golpe que alcanza — sin este gate, un ataque de 0 contra jugador sin Bloqueo
        // consumia el pierce y disparaba los listeners (pierce fantasma, audit 2026-07-04).
        if (result.UnblockedDamage + result.BlockedDamage <= 0) return;
        // Golpe anulado por la CORONA (audit 2026-07-04): la Corona devuelve 0 en el hook de HP y el
        // motor lo computa WasFullyBlocked (unblocked==0 con Bloqueo de por medio) — el early-return
        // de abajo se lo tragaba, el pierce jamas se consumia y la Corona anulaba TODOS los golpes
        // parcialmente bloqueados del turno. Si el cupo del pierce y el de la Corona siguen libres,
        // tratamos este golpe como el que ALCANZO (consume ambos cupos via Broadcast).
        var crownAnnulled = false;
        if (result.WasFullyBlocked && FgoCombatState.GetTurn(Owner.Creature, 2) == 0 && !IsPierceSuppressed())
        {
            foreach (var p in FGOCore.FGOCoreCode.Listeners.PowersOf<SiegfriedSaber.SiegfriedSaberCode.Powers.PeerlessCrownPower>(Owner.Creature))
            {
                if (p.HasFreeCharge) { crownAnnulled = true; break; }
            }
        }
        if (result.WasFullyBlocked && !crownAnnulled) return; // no te alcanzó (espejo del amount>0 del power)
        // Tarnkappe (IDragonScalePierceSuppressor): la espalda cubierta corta el pierce Y toda vía de NP que
        // el golpe-que-alcanza alimentaría (el +5 NP de abajo y el broadcast del pierce a los listeners).
        if (IsPierceSuppressed()) return;

        // Espeja la decisión del power para ESTE golpe que alcanza: si el cupo seguía libre,
        // el power dejó que el golpe atravesara 1 escama (espalda expuesta) → consumilo. Acá, en el camino REAL
        // del daño (no en la lectura pura ShouldPierceScales, que un preview podría disparar),
        // avisamos a los listeners del pierce (p.ej. la carta Cicatriz del Tilo). La reliquia
        // no añade efecto propio al pierce; los riders viven en los listeners.
        if (FgoCombatState.GetTurn(Owner.Creature, 2) == 0)
        {
            await FgoCombatState.SetTurn(
                choiceContext, Owner.Creature, 2, 1, cardSource);
            await DragonScalesPierce.Broadcast(Owner.Creature, choiceContext);
        }

        // La batería es fácil de leer: cada ataque powered que realmente quita Vida paga +5 NP.
        // Los golpes anulados no pagan y el límite compartido evita abusos con multi-hit.
        if (result.UnblockedDamage >= 1 &&
            FgoCombatState.GetTurn(Owner.Creature, 3, 2) < NpProcCapPerTurn)
        {
            await FgoCombatState.IncrementTurn(
                choiceContext, Owner.Creature, 3, NpProcCapPerTurn, cardSource, width: 2);
            Flash();
            await NpCharge.Gain(choiceContext, Owner.Creature, NpPerHpHit, null);
        }
    }

    // ¿Hay un power que cubre la espalda expuesta (Tarnkappe)? Lectura pura, preview-safe.
    private bool IsPierceSuppressed()
        => Listeners.PowersOf<IDragonScalePierceSuppressor>(Owner.Creature).Any(suppressor => suppressor.SuppressPierce);
}

/// <summary>
/// Orobas replaces the starter with a fresh Ancient instance. Preserve the extra starting Scale
/// progression just like FGOCore already preserves the shared NP level and pity properties.
/// </summary>
[HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Replace))]
internal static class LindenLeafReplacementStatePatch
{
    [HarmonyPrefix]
    private static void CaptureStartingScales(RelicModel original, RelicModel replace, out int? __state)
    {
        __state = original is LindenLeaf source && replace is LindenLeaf
            ? source.StartingScales
            : null;
    }

    [HarmonyPostfix]
    private static void RestoreStartingScales(ref Task<RelicModel> __result, int? __state)
    {
        if (__state is not { } startingScales) return;
        __result = AwaitAndRestore(__result, startingScales);
    }

    private static async Task<RelicModel> AwaitAndRestore(
        Task<RelicModel> replacementTask,
        int startingScales)
    {
        var replacement = await replacementTask;
        if (replacement is LindenLeaf target)
        {
            target.StartingScales = startingScales;
        }
        return replacement;
    }
}
