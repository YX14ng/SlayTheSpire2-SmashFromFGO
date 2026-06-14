using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Basic;

/// <summary>
/// FIRMA «Recuperar el Aliento» (调息 / Catch Your Breath) — la firma básica que enseña que
/// DEFENDER = RESPIRAR (DESIGN-OKITA §5.1): 5 Bloqueo; +2 *Aliento (up: 8 Bloqueo; +2 Aliento).
/// El engawa de la última primavera. Recupera el recurso que las *Ráfagas gastan.
/// </summary>
public sealed class CatchYourBreath() : OkitaCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Breath", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await Aliento.Gain(Owner.Creature, DynamicVars["Breath"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m); // 5 -> 8
}
