using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Núcleo de Avalon — Habilidad 1⚡, Exhaust: Carga NP +50. Mejora: +70.
/// Co-op: cada aliado gana Carga NP +10.
/// </summary>
public sealed class AvalonCore() : ArtoriaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 50),
        new DynamicVar("AllyNpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);

        // Co-op: el núcleo de Avalon carga a todo el party.
        await ForEachAlly(async ally =>
            await NpCharge.Gain(choiceContext, ally, DynamicVars["AllyNpCharge"].IntValue, this));
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
