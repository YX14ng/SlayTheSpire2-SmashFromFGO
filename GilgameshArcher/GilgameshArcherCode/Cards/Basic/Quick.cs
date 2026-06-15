using GilgameshArcher.GilgameshArcherCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Basic;

/// <summary>
/// Quick / 绿卡 (DESIGN-GILGAMESH §5.1) — FIRMA básica del mazo inicial QAABB. La carta de comando verde
/// que ENSEÑA el hilo de las Estrellas de Crítico: a 100★, FGOCore auto-paga 1 *Crítico Listo (próximo
/// Ataque ×2 — el juicio despectivo del Rey). Un disparo veloz que acumula la luz del tesoro. Lleva el
/// tag Strike heredado de la base (compat de eventos, patrón Morgan/Siegfried).
///
/// 6 de daño + 30 Estrellas de Crítico (up +3 daño / +20 Estrellas). Reusa <c>CritStarsPower</c> de
/// FGOCore (auto-proc a 100 → Crítico Listo, igual que Mash). Números exactos §5.1.
/// </summary>
public sealed class Quick() : GilgameshCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    private const int StarsGain = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Stars", StarsGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Stars"].UpgradeValueBy(20m);
    }
}
