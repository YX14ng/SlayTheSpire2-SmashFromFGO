using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Common;

/// <summary>Llave Girada (转动之钥) — DESIGN-GILGAMESH §5.2. 1⚡ Hab: +20 Carga NP + robá 1 (up +30 NP).
/// NP + ciclo (slot 高速咏唱). El up sube SOLO el NP; el robo queda fijo. Patrón RideTheStolen
/// (NpCharge.Gain + CardPileCmd.Draw).</summary>
public sealed class TurnedKey() : GilgameshCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Np", 20), new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(10m);
}
