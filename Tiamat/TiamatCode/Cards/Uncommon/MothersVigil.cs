using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Vigilia de la Madre — Poder poco común: concede <see cref="MothersVigilPower"/> (al inicio de tus
/// turnos, si tenés 3+ Laḫmu, robás 1 por acumulación). Robo gateado por población: premia mantener
/// el enjambre lleno en vez de devorarlo todo. También suma al hueco Poco común/Poder=1 que rompía
/// el evento de pociones.
/// </summary>
public sealed class MothersVigil() : TiamatCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stacks", 1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MothersVigilPower>(), HoverTipFactory.FromPower<LahmuSwarmPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MothersVigilPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stacks"].UpgradeValueBy(1m);
}
