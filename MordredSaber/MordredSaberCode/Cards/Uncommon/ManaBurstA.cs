using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Estallido de Maná A (魔力放出A) — DESIGN-MORDRED §5.2. 1⚡ Hab, Exhaust: este turno tus Ataques
/// hacen +4 (up +6). El S1 base 1:1 (Buster +50% un turno → Fuerza temporal este turno); el Exhaust
/// es el cooldown (un uso/combate por copia). Aplica <see cref="ManaBurstStrengthPower"/> (Fuerza
/// temporal que se auto-remueve a fin de turno). Patrón DragonHunterStrike (rank-up vía Fuerza temporal).
/// </summary>
public sealed class ManaBurstA() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Strength", 4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ManaBurstStrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Strength"].UpgradeValueBy(2m);
}
