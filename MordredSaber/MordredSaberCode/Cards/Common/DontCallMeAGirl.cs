using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Common;

/// <summary>
/// «¡No me llames niña!» (别叫我小姑娘！) — DESIGN-MORDRED §5.1, carta-meme (las reglas de trato del
/// perfil oficial). 0⚡ Hab, Exhaust: +10 Estrellas + 10 de Carga NP (up: +20★/+10 NP). Doble-feeder de
/// 0⚡ con Exhaust como cooldown (el berrinche se grita una vez). Patrón GraniStalk (★) + ManaIgnition
/// (NP) con CardKeyword.Exhaust de GatherProwess. El up sube SOLO el ★ (el +NP queda en su denominación).
/// </summary>
public sealed class DontCallMeAGirl() : MordredCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
