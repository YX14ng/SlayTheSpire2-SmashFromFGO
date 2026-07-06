using FGOCore.FGOCoreCode.Block;
using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>Crisálida Abisal — el botón de pánico de Lily: 18 Baluarte (retenido) + 2 Crianza, Exhaust.
/// Tasa: Impervious vanilla es 30 bloqueo normal a 2⚡ con Exhaust; acá el bloqueo es Baluarte (se
/// retiene, capado por <see cref="BlockRetention"/>) y trae rider de Crianza, así que baja a 18 (§3).
/// La Madre se encapulla y el enjambre madura. También suma al hueco Rara/Habilidad=2 que rompía el
/// evento de pociones.</summary>
public sealed class AbyssalChrysalis() : TiamatCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(18m, ValueProp.Move),
        new DynamicVar("Nurture", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue);
        await Lahmu.Feed(Owner.Creature, DynamicVars["Nurture"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(6m);
        DynamicVars["Nurture"].UpgradeValueBy(1m);
    }
}
