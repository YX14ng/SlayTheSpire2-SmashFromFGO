using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// La Hoja de Tilo (菩提叶之弱点) — reliquia starter de Siegfried y motor de su identidad.
/// Al iniciar cada combate otorga <see cref="DragonScalesPower"/> (Sangre de Dragón) inicial.
/// Su gancho <see cref="IDragonScalePiercer"/> hace que el PRIMER golpe que te ALCANZA cada
/// turno IGNORE las escamas (la espalda expuesta — la debilidad canónica hecha regla).
/// Cuando un golpe SÍ reducido por las escamas aún inflige ≥1 (P2: la armadura no trabajó
/// gratis), refunda +5 NP, con tope de 3 procs/turno (P1/P3).
///
/// Sincronía con DragonScalesPower (orden del pipeline: ModifyHpLostBeforeOsty del power →
/// LoseHp → AfterDamageReceived de esta reliquia, por golpe): el power consulta
/// <see cref="ShouldPierceScales"/> como LECTURA PURA (a prueba de previews de daño); el
/// consumo del cupo 1/turno ocurre aquí, en el camino real del daño.
/// </summary>
public sealed class LindenLeaf : SiegfriedRelic, IDragonScalePiercer
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const int StartingScales = 2;   // SdD inicial (§3)
    private const int NpPerReducedHit = 5;   // +NP por golpe reducido con residual ≥1 (§8.1)
    private const int NpProcCapPerTurn = 3;  // tope agregado del trigger defensivo (P1/P3)

    private bool _piercedThisTurn;
    private int _npProcsThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    // LECTURA PURA: no muta estado (el consumo va en AfterDamageReceived). Pasa el primer
    // golpe que alcanza cada turno; los siguientes chocan contra las escamas.
    public bool ShouldPierceScales(ValueProp props, Creature? dealer)
    {
        if (IsPierceSuppressed()) return false; // Tarnkappe: la espalda ya no está expuesta
        return !_piercedThisTurn;
    }

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        _piercedThisTurn = false;
        _npProcsThisTurn = 0;
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, StartingScales, Owner.Creature, null);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        if (side == CombatSide.Player)
        {
            _piercedThisTurn = false;
            _npProcsThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner.Creature) return;
        if (!props.IsPoweredAttack()) return;
        if (result.WasFullyBlocked) return; // no te alcanzó (espejo del amount>0 del power)
        // Tarnkappe (IDragonScalePierceSuppressor): la espalda cubierta corta el pierce Y toda vía de NP que
        // el golpe-que-alcanza alimentaría (el +5 NP de abajo y el broadcast del pierce a los listeners).
        if (IsPierceSuppressed()) return;

        // Espeja la decisión del power para ESTE golpe que alcanza: si el cupo seguía libre,
        // el power dejó pasar el golpe (espalda expuesta) → consumilo. Acá, en el camino REAL
        // del daño (no en la lectura pura ShouldPierceScales, que un preview podría disparar),
        // avisamos a los listeners del pierce (p.ej. la carta Cicatriz del Tilo). La reliquia
        // no añade efecto propio al pierce; los riders viven en los listeners.
        if (!_piercedThisTurn)
        {
            _piercedThisTurn = true;
            await DragonScalesPierce.Broadcast(Owner.Creature, choiceContext);
            return;
        }

        // El golpe chocó contra las escamas. P2: solo refunda NP si AÚN infligió ≥1 tras la SdD
        // (un golpe anulado del todo = la armadura trabajó gratis, sin NP).
        if (result.UnblockedDamage >= 1 && _npProcsThisTurn < NpProcCapPerTurn)
        {
            _npProcsThisTurn++;
            Flash();
            await NpCharge.Gain(Owner.Creature, NpPerReducedHit, null);
        }
    }

    // ¿Hay un power que cubre la espalda expuesta (Tarnkappe)? Lectura pura, preview-safe.
    private bool IsPierceSuppressed()
    {
        foreach (var power in Owner.Creature.GetPowerInstances<PowerModel>())
        {
            if (power is IDragonScalePierceSuppressor suppressor && suppressor.SuppressPierce) return true;
        }
        return false;
    }
}
