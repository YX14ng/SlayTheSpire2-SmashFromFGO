using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Festín tras la Batalla (战后宴席) — DESIGN-MORDRED §5.3. 1⚡ Hab, Exhaust: curás 10 + 10 de Carga NP
/// (up: 14/+20). La cena con Sisigou: el sustain conectado al hilo NP. Exhaust como cooldown. Patrón
/// ChaldeaSandwich (HealVar) + NpCharge.Gain. Sube AMBOS con el up.
/// </summary>
public sealed class FeastAfterBattle() : MordredCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(10m), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
