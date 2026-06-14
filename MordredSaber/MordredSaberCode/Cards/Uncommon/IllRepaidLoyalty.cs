using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Lealtad Mal Pagada (错付的忠诚) — DESIGN-MORDRED §5.2. 1⚡ Hab, Exhaust: curás 6 + 10 de Carga NP
/// (up: 9 / +20). Sustain conectado al hilo NP (su lealtad al Rey, nunca correspondida). El Exhaust es
/// el cooldown del curado. Patrón Palingenesis/MapoTofu (HealVar) + carga NP.
/// </summary>
public sealed class IllRepaidLoyalty() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(6m), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
