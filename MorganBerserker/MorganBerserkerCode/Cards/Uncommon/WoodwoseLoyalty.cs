using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Lealtad de Woodwose (伍德沃斯的忠诚) — RE-POOL V2 [NUEVA] (injerto de P1): Habilidad 1⚡:
/// 8 de Bloqueo; en Reina Hada o Reina del Invierno: ese Bloqueo se conserva hasta tu próximo
/// turno (mejora 12). El woodwose leal de Faerie Britain monta guardia mientras la Reina juzga.
/// Glow con la forma correcta.
/// </summary>
public sealed class WoodwoseLoyalty() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<WoodwoseLoyaltyPower>()];

    private bool InQueenForm =>
        Owner.Creature.HasPower<FairyQueenFormPower>() || Owner.Creature.HasPower<WinterQueenFormPower>();

    protected override bool ShouldGlowGoldInternal => InQueenForm;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var gained = await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (gained > 0 && InQueenForm)
        {
            await PowerCmd.Apply<WoodwoseLoyaltyPower>(choiceContext, Owner.Creature, gained, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
