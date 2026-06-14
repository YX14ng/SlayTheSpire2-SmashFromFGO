using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>Cadena del Cielo: Enkidu (天之锁·恩奇都) — DESIGN-GILGAMESH §5.4, anti-divino del lore (la cadena
/// que ata dioses: cuanto más divino, más fuerte ata). 2⚡ Hab Exhaust: aplica 2 de Vulnerable; contra
/// Élites/Jefes (<see cref="RoyalTrait.IsDivine"/>): aplica 4 de Vulnerable EN SU LUGAR y el enemigo
/// pierde 2 de Fuerza (StrengthPower negativo). Glow vs Élite/Jefe. up 3/5/3.</summary>
public sealed class ChainsOfHeavenEnkidu() : GilgameshCard(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>("Vulnerable", 2m),
        new PowerVar<VulnerablePower>("DivineVulnerable", 4m),
        new PowerVar<StrengthPower>("StrengthLoss", 2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<StrengthPower>()];

    protected override bool ShouldGlowGoldInternal =>
        Owner.Creature.CombatState?.RunState.CurrentRoom?.RoomType is
            MegaCrit.Sts2.Core.Rooms.RoomType.Elite or MegaCrit.Sts2.Core.Rooms.RoomType.Boss;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (RoyalTrait.IsDivine(cardPlay.Target))
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["DivineVulnerable"].BaseValue, Owner.Creature, this);
            await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -DynamicVars["StrengthLoss"].BaseValue, Owner.Creature, this);
        }
        else
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);
        DynamicVars["DivineVulnerable"].UpgradeValueBy(1m);
        DynamicVars["StrengthLoss"].UpgradeValueBy(1m);
    }
}
