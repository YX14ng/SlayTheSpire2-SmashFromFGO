using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Truco de Robin Goodfellow (好人罗宾的恶作剧 / Robin Goodfellow's Trick) — DESIGN-OBERON §6.3. 0⚡
/// Habilidad: alterná El Rey del Cuento ↔ El Príncipe del Invierno; robá 1 (up robá 2). El switch premium
/// (patrón Liebre Blanca). En Vortigern (permanente) el toggle no hace nada.
/// </summary>
public sealed class RobinGoodfellowsTrick() : OberonCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Draw", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<WinterPrincePower>())
        {
            await FormSwitch.Enter<StorybookKingPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await FormSwitch.Enter<WinterPrincePower>(choiceContext, Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
