using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>Abrazo de la Dama del Lago (水妃的拥抱) — 12 de Bloqueo; solo jugable en forma Bruja.</summary>
public sealed class EmbraceOfTheLake() : MorganCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(12m, ValueProp.Move)];

    protected override bool IsPlayable =>
        Owner.Creature.HasPower<RainWitchFormPower>() || Owner.Creature.HasPower<WinterQueenFormPower>();

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
    }
}
