using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Marea Voraz — EL loop ofensivo de Lily (DESIGN-REVIEW-2 §3). Concede <see cref="TidalSwarmPower"/>:
/// de ahora en más, el enjambre muerde TAMBIÉN al final de CADA turno tuyo (no solo en el turno
/// enemigo) al más maldito. Le da dientes a la fase Lily sin tocar el knob global del enjambre en
/// FGOCore. Apilable: cada copia suma otra mordida de fin de turno.
/// </summary>
public sealed class RavenousTide() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bites", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<TidalSwarmPower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TidalSwarmPower>(choiceContext, Owner.Creature, DynamicVars["Bites"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Bites"].UpgradeValueBy(1m);
}
