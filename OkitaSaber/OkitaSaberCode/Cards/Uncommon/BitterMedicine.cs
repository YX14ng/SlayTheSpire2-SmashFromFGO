using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Medicina Amarga (苦药) — DESIGN-OKITA §5.3. 1⚡ Hab, Exhaust: curá 5; exhaustá todas las *Tos de
/// tu mano (up: curá 8). Su única cura: chica + Exhaust (FGO: sin curas). Lee la mano vía
/// PlayerCombatState.Hand y exhausta con CardCmd.Exhaust.
/// </summary>
public sealed class BitterMedicine() : OkitaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(5m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        if (Owner.PlayerCombatState == null) return;
        foreach (var tos in Owner.PlayerCombatState.Hand.Cards.OfType<Tos>().ToList())
        {
            await CardCmd.Exhaust(choiceContext, tos);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Heal.UpgradeValueBy(3m); // curá 5 -> 8
}
