using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Canto del Atardecer (黄昏之歌 / Dusk Song) — DESIGN-OBERON §6.2. 1⚡ Habilidad: 6 de Bloqueo;
/// +10 Carga NP (up +3 Bloqueo / +5 NP). El defensivo que también empuja el medidor: el plan B que no
/// frena el motor. El up sube ambos.
/// </summary>
public sealed class DuskSong() : OberonCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(6m, ValueProp.Move), new DynamicVar("Np", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["Np"].UpgradeValueBy(5m);
    }
}
