using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Llamado del Mar de Vida — el acelerador puro de Sobrecarga (REDESIGN-TIAMAT §47). Obtenés
/// !NpCharge! de Carga NP y robás !Draw! carta. El Mar responde al llamado: empuja el medidor
/// hacia la ventana Génesis y repone la mano para seguir sembrando.
/// </summary>
public sealed class CallOfTheSea() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 18),
        new DynamicVar("Draw", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (Owner.Creature.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner.Creature.Player);
        }
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(6m);
}
