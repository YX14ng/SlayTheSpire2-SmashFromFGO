using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Manipulación de la Espada Sagrada A (聖剣操作, skill real de Castoria) — Poder:
/// al jugarla ganás 1 Anti-Purga; al inicio de tu turno: ganás 1★. Mejorada: cuesta 1⚡.
/// </summary>
public sealed class SacredSwordControl() : ArtoriaCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<SacredSwordControlPower>("Stacks", 1m),
        new DynamicVar("AntiPurge", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AntiPurgePower>(Owner.Creature, DynamicVars["AntiPurge"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<SacredSwordControlPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
