using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Rhongomyniad, el Cetro de la Reina (止境之枪·王笏) — starter relic, rediseño v2
/// (lección 焰刑地狱: la starter convierte eventos universales en recursos del kit):
/// (1) MANTIENE: at combat start enter Fairy Queen form (and kick off FormVisuals'
/// background preload) — without it Morgan fought FORMLESS until her first switch.
/// (2) MANTIENE: the first time you change form each combat: +1 Energy, draw 1
/// and NP +10 (makes the first switch tempo-positive).
/// (3) AGREGA (rediseño 2026-06-15: swap Estrellas→Maldición): every time Morgan loses HP
/// (any source — enemy attacks, self-damage): apply 3 Curse to a random living enemy, capped
/// at 3 events per turn (parche P2; mismo patrón _triggersThisTurn que MadnessEnhancementPower).
/// Sangrar → SEMBRAR la bomba: el daño propio de Morgan (MadLunge/TyrantsBlood/FaeBloodPact)
/// y los golpes que tanquea alimentan directamente la Maldición que la Reina cosecha. Parche
/// P4: el tick de FaeBloodPact NO cuenta. Patrón de enemigo aleatorio calcado de BottledMors.
/// </summary>
public sealed class QueensScepter : MorganRelic, IFormChangeListener
{
    public const int NpOnFirstSwitch = 10;
    public const int CursePerHpLoss = 3;
    public const int CurseTriggersPerTurn = 3;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<WorldsEndCoronation>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    public override async Task BeforeCombatStartLate()
    {
        // Forma inicial: Reina. source == null -> no cuenta como "cambio de forma".
        await FormSwitch.Enter<Powers.Forms.FairyQueenFormPower>(null, Owner.Creature, null);
    }

    /// <summary>(4) 2026-07-04, feedback de jugadores ("¿puedo entrar a Caster desde el inicio?"):
    /// al empezar tu PRIMER turno de cada combate, manifiesta una Metamorfosis de la Reina gratis
    /// (0⚡, Etérea + Exhaust — alterna Reina ↔ Bruja). Garantiza el acceso a la forma sembradora
    /// desde el turno 1 sin diluir el mazo; usarla dispara el bono (2) de primer cambio.</summary>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner || FgoCombatState.GetCombat(Owner.Creature, 1) != 0) return;
        await FgoCombatState.SetCombat(choiceContext, Owner.Creature, 1, 1);
        await FGOCore.FGOCoreCode.Combat.ManifestCards.ManifestToHand<Cards.Special.QueensMetamorphosis>(Owner.Creature, 1.0f);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        // Tope P2 por RONDA: se resetea al inicio del turno del jugador y cuenta
        // tanto el autodaño propio como los golpes tanqueados en el turno enemigo.
        // FgoTurnStatePower resets the synchronized counter for participating owners.
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature || result.UnblockedDamage <= 0) return;
        if (Powers.FaeBloodPactPower.TickInProgress) return; // P4: el tick del Pacto no siembra Maldición.

        if (Owner.Creature.CombatState is not { } combatState) return;
        var living = new List<Creature>();
        foreach (var enemy in combatState.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead) living.Add(enemy);
        }
        if (living.Count == 0) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 5, 2) >= CurseTriggersPerTurn) return;
        await FgoCombatState.IncrementTurn(
            choiceContext, Owner.Creature, 5, CurseTriggersPerTurn, cardSource, width: 2);

        Flash();
        var victim = living[Owner.RunState.Rng.CombatCardGeneration.NextInt(living.Count)];
        // applier = Owner.Creature para que los amplificadores de Maldición (Caster/Invierno) cuenten.
        await Curses.Apply(choiceContext, victim, CursePerHpLoss, Owner.Creature, null);
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (FgoCombatState.GetCombat(Owner.Creature, 0) != 0) return;
        var context = choiceContext ?? new BlockingPlayerChoiceContext();
        await FgoCombatState.SetCombat(context, Owner.Creature, 0, 1);
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
        await NpCharge.Gain(context, Owner.Creature, NpOnFirstSwitch, null);
        if (choiceContext != null)
        {
            if (Owner.Creature.Player?.PlayerCombatState is not { } playerCombatState) return;
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
            // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
            // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
            // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
            var inDeck = playerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            if (inDeck > 0)
            {
                await CardPileCmd.Draw(choiceContext, 1, Owner);
            }
        }
    }
}
