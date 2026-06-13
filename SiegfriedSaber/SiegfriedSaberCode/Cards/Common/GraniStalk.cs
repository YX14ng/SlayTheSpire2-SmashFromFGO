using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Stars;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Acecho de Grani (格拉尼的潜行 / Grani's Stalk) — DESIGN-SIEGFRIED §7. 0⚡: obtené 10
/// Estrellas de Crítico (up 20). UN solo rider, sin +NP (P10: nunca tocar el daño base de
/// la ult ni meter +NP gratis por frecuencia). Grani, el corcel de Siegfried, ronda el campo
/// y abre la línea de visión para el golpe certero. Reusa CritStarsPower de FGOCore (auto-proc
/// a 100 → Crítico Listo ×2, igual que Mash). Patrón copiado de QuickMorgan/WildHuntCharge.
///
/// Balance (SKILL §2 "Estrellas del Regent": 1★ ≈ 3-5 daño ≈ ½⚡; escala-ecosistema ÷10, o sea
/// 10★-ecosistema ≈ 1★-Regent ≈ ½⚡): +10★ a 0⚡ ≈ ½⚡ de valor → generador puro de banca de
/// 0⚡ con UN rider, dentro del slot 0⚡ Común (§2: "Común 0⚡: 3-6 daño"). El up a +20★ ≈ 1⚡
/// (techo §1.bis: tasa cruda ×1.25, no se cruza — sigue siendo ½ generador).
/// </summary>
public sealed class GraniStalk() : SiegfriedCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}
