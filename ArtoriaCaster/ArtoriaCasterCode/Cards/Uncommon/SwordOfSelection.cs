using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Espada de Selección EX (skill real S3 de Castoria) — 1 de ANTI-PURGA; tu próximo
/// Ataque este turno hace +10 de daño (power interno <see cref="NextAttackBoostPower"/>).
/// Exhaust.
/// </summary>
public sealed class SwordOfSelection() : ArtoriaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AntiPurge", 1),
        new PowerVar<NextAttackBoostPower>("Boost", 10m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<NextAttackBoostPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, DynamicVars["AntiPurge"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<NextAttackBoostPower>(Owner.Creature, DynamicVars["Boost"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Boost"].UpgradeValueBy(5m);
    }
}
