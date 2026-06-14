using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// PODER «Genio de la Espada» (剑之天才) — DESIGN-OKITA §5.4. 2⚡ Poder: aplica
/// <see cref="SwordGeniusPower"/> con Amount 8 (tus CRÍTICOS hacen +8) (up: +12). Slot Magia Única
/// (crit dmg engine). El Amount del power (Counter) es el bono de daño crítico.
/// </summary>
public sealed class SwordGenius() : OkitaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SwordGeniusPower>("CritBonus", 8m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SwordGeniusPower>(Owner.Creature, DynamicVars["CritBonus"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["CritBonus"].UpgradeValueBy(4m); // +8 -> +12
}
