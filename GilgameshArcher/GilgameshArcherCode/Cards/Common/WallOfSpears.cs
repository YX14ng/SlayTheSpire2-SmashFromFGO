using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>Muro de Lanzas (长枪之壁) — DESIGN-GILGAMESH §5.2, Armas + defensa. 1⚡ Hab: 8 de Bloqueo; si
/// jugaste un Arma del Tesoro este turno: +3 (up 11 / +4). Glow cuando jugaste un Arma. Lee
/// <see cref="ArmsPlayedPower"/> (0 hasta que el Arsenal lo alimente). El up sube ambos.</summary>
public sealed class WallOfSpears() : GilgameshCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new DynamicVar("Bonus", 3)];

    protected override bool ShouldGlowGoldInternal => ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0 ? DynamicVars["Bonus"].IntValue : 0;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + bonus, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
