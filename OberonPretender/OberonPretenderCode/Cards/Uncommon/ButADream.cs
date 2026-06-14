using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using OberonPretender.OberonPretenderCode.Extensions;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Solo un Sueño (不过是个梦 / But a Dream) — DESIGN-OBERON §6.3 ("no more yielding but a dream"). 1⚡
/// Habilidad · Exhaust: remové tus debuffs; +10 Carga NP (up: además 1 Artefacto). El cleanse permitido
/// por la regla P8 (Exhaust paga el perdón), vía <see cref="OberonExtensions.RemoveAllDebuffs"/>.
/// </summary>
public sealed class ButADream() : OberonCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Charge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await OberonExtensions.RemoveAllDebuffs(Owner.Creature);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<ArtifactPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }
}
