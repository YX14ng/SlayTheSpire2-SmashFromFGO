using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Resistencia Mágica E (魔力抵抗E, KIT) — DESIGN-OKITA §5.3. 1⚡ Hab: 1 Artifact; +10 Carga NP
/// (up: 2 Artifact). El rango E como gag ("al menos lo intenta").
/// </summary>
public sealed class MagicResistanceE() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<ArtifactPower>("Artifact", 1m), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ArtifactPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ArtifactPower>(Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m); // 1 -> 2
}
