using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Sello de Comando de Kairi (凯利的令咒) — DESIGN-MORDRED §5.3. 0⚡ Hab, Exhaust: +50 de Carga NP
/// (up +100). Slot Última Orden: su Master ordena, el medidor salta de golpe. Burst de NP de un saque,
/// Exhaust como cooldown (un uso/combate). Patrón LastWill. El up sube el NP (no toca el coste).
/// </summary>
public sealed class KairisCommandSeal() : MordredCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(50m);
}
