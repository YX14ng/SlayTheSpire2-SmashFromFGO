using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Camlann (卡姆兰) — DESIGN-MORDRED §5.3. 1⚡ Hab, Exhaust: ganás 1 de ALZARSE (Guts); cuando se
/// activa (un golpe te mataría → sobrevivís a 1 de Vida), +100 NP (up: y +50 Estrellas). El lore beat
/// de la batalla final: caer es el power-spike. Aplica <see cref="CamlannGutsPower"/> (subclase de
/// GutsPower de FGOCore). El +100 NP solo paga si Alzarse llega a gatillar (§9: condición que puede no
/// llegar). El up habilita el +★ (fija Upgraded en el power).
/// </summary>
public sealed class Camlann() : MordredCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<GutsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<CamlannGutsPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.Upgraded = IsUpgraded;
    }
}
