using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Limo Protector — Poder poco común: concede <see cref="ProtectiveSiltPower"/> (+3 Baluarte al inicio
/// de tus turnos por acumulación). El Metallicize de Tiamat, en Baluarte: defensa pasiva que se retiene
/// y sostiene la fase Lily de tempo-control. También suma al hueco Poco común/Poder=1 que rompía el
/// evento de pociones.
/// </summary>
public sealed class ProtectiveSilt() : TiamatCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Block", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ProtectiveSiltPower>(), HoverTipFactory.FromPower<BulwarkPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ProtectiveSiltPower>(choiceContext, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}
