using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// #18 Canto de la Lluvia (雨之咏唱) — rediseño v2: el ACCESO COMÚN a la forma Bruja
/// que faltaba (regla §5 de formas: barata + efecto inmediato chico). 1⚡: entrá en
/// forma Bruja de la Lluvia y obtené 5 de Bloqueo. (up: Bloqueo 5→8)
/// </summary>
public sealed class RainChant() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await FormSwitch.Enter<RainWitchFormPower>(choiceContext, Owner.Creature, this);
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
