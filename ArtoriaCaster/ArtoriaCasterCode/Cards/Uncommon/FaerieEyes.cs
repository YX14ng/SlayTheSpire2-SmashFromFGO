using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Ojos Feéricos (妖精眼, pasiva real del Berserker) — Poder: cada golpe enemigo
/// anulado por completo: ganás 1★ y Carga NP +5. Mejorada: 1★ y +8.
/// </summary>
public sealed class FaerieEyes() : ArtoriaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<FaerieEyesPower>("Power", 1m),
        new DynamicVar("Stars", FaerieEyesPower.StarsPerTrigger),
        new DynamicVar("NpCharge", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CriticalStarsPower>(), HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<AntiPurgePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<FaerieEyesPower>(Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
        var power = Owner.Creature.GetPowerInstances<FaerieEyesPower>().FirstOrDefault();
        if (power != null)
        {
            power.NpPerTrigger = DynamicVars["NpCharge"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(3m);
    }
}
