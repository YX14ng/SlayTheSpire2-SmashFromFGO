using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Golpe de Ea (乖离剑之击) — DESIGN-GILGAMESH §5.4, el preludio de Enuma (Ea sin recitar). 2⚡ At:
/// 18 de daño; contra Élites/Jefes (<see cref="RoyalTrait.IsDivine"/>) +8; +10 Carga NP (up 24 / +10).
/// Glow vs Élite/Jefe. El up sube SOLO la base.</summary>
public sealed class StrikeOfEa() : GilgameshCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(18m, ValueProp.Move), new DynamicVar("Divine", 8), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => RoyalTrait.IsInDivineRoom(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var divineBonus = RoyalTrait.IsDivine(cardPlay.Target) ? DynamicVars["Divine"].IntValue : 0;
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue + divineBonus);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(6m);
}
