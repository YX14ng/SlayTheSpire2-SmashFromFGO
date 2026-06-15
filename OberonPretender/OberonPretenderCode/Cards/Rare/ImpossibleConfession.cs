using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Extensions;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Confesión Imposible (无法说出口的告白 / Impossible Confession) — DESIGN-OBERON §6.4. 1⚡ Habilidad ·
/// Exhaust: curás 8; removés tus debuffs (*«No tengo nada que me guste» — también mentira*) (up cura 12).
/// El cleanse reusa <see cref="OberonExtensions.RemoveAllDebuffs"/> (API Cleanse de FGOCore), que preserva
/// la Deuda (IResourcePower) y los recursos; antes el filtro inline borraba la Deuda por error.
/// </summary>
public sealed class ImpossibleConfession() : OberonCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(8m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        await OberonExtensions.RemoveAllDebuffs(Owner.Creature);
    }

    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(4m);
}
