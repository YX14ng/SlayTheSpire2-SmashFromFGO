using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Cabalgar E (骑乘E, KIT) — DESIGN-OKITA §5.3. 1⚡ Poder: aplica <see cref="RidingEPower"/> (al inicio
/// de cada turno: +5★) (up: +10). El goteo mínimo del rango E ("sabe montar… apenas").
/// </summary>
public sealed class RidingE() : OkitaCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RidingEPower>("Stars", 5m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RidingEPower>(Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(5m); // +5 -> +10
}
