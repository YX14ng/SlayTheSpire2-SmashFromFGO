using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Lágrimas tras la Sonrisa — Habilidad 1⚡, Exhaust: curás 8; removés tus debuffs.
/// Mejora: 12.
/// </summary>
public sealed class TearsBehindTheSmile() : ArtoriaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(8m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await RemoveOwnDebuffs();
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(4m);
    }
}
