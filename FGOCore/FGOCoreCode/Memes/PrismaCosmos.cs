using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Prisma Cosmos (棱镜星云) — Poder: al inicio de cada turno ganás 20 de Carga NP. El CE de
/// carga sostenida clásico (motor de generación pasiva, ~½⚡ de valor por turno). Rara: en
/// peleas largas (HextechRunes) construye un loop de ultis.
/// </summary>
public sealed class PrismaCosmos() : MemeCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<PrismaCosmosPower>("NpCharge", 20m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PrismaCosmosPower>(choiceContext, Owner.Creature, DynamicVars["NpCharge"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(5m);
    }
}
