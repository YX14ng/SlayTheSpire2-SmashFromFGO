using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// Caleidoscopio (万华镜) — el 礼装 de arranque explosivo: al jugarla, +80 de Carga NP.
/// 80 NP ≈ casi una manifestación de ulti (umbral 100) de un solo golpe — Exhaust + Rara lo
/// pagan (es un burst de setup, no un loop). Sin daño: 0⚡ con Exhaust es el slot de un
/// generador de recurso premium (cf. Comet 0⚡+5★).
/// </summary>
public sealed class Kaleidoscope() : MemeCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 80)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
