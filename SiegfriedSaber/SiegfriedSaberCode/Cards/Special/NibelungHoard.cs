using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

/// <summary>
/// Hort der Nibelungen / El Tesoro de los Nibelungos (尼伯龙根的宝藏) — Ancient. 0⚡ Exhaust: poder enorme
/// (+5 SdD y +80 NP, up +100) que NO podés devolver: el oro del dragón está maldito → agrega permanentemente
/// un Arrepentimiento (Maldición vanilla) a tu MAZO. El costo de la Maldición permanente NO se quita con el up.
/// </summary>
public sealed class NibelungHoard() : SiegfriedCard(0, CardType.Skill, CardRarity.Ancient, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scales", 5), new DynamicVar("Np", 80)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonScalesPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        await CardPileCmd.AddCurseToDeck<Regret>(Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(20m);
}
