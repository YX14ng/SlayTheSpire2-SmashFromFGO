using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Póster GUDAGUDA con Nobu (与信长的GUDAGUDA海报) — reliquia de TIENDA (DESIGN-OKITA §6.2): la PRIMERA
/// vez que tu *Aliento llega a 0 cada combate, +<see cref="Stars"/> *Estrellas. (¡El drama es glorioso!)
///
/// El agotamiento de Aliento ya está marcado por AlientoPower.HitZeroThisTurn (lo setea Aliento.Spend
/// al tocar 0, con cap 1/turno). Lo leemos al final de tu turno (patrón GreyCatOfTrifas) con un flag
/// ESTRUCTURAL de 1 proc/combate — sin "vigilar" números, sin polling de daño.
/// </summary>
public sealed class GudagudaPoster : OkitaRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;

    private const int Stars = 50;

    private bool _firedThisCombat;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AlientoPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override Task BeforeCombatStartLate()
    {
        _firedThisCombat = false;
        return base.BeforeCombatStartLate();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player || _firedThisCombat) return;
        var breath = Aliento.Power(Owner.Creature);
        // Aliento.Spend remueve el power al tocar 0 (Amount 0), por eso comprobamos ambos: la marca
        // del turno (si el power sigue vivo) o que el contador esté efectivamente en 0.
        var hitZero = breath?.HitZeroThisTurn == true || Aliento.Of(Owner.Creature) <= 0;
        if (!hitZero) return;
        _firedThisCombat = true;
        Flash();
        await CritStars.Gain(Owner.Creature, Stars, null);
    }
}
