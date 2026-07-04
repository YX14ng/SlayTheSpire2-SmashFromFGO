using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Velocidad Cegadora (神速) — DESIGN-OKITA §5.3. 0⚡ Hab, RÁFAGA 2: +1⚡; robá 1 (up: RÁFAGA 1).
/// Aliento → energía, capeado por el income de Aliento. La mejora baja el coste de Aliento (el
/// <see cref="RafagaCost"/> es un campo que el up reduce a 1). Paga el segundo coste vía
/// <see cref="Rafaga.Pay"/>.
/// </summary>
public sealed class BlindingSpeed() : OkitaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IRafagaCard
{
    // DynamicVar (audit 2026-07-04): con el coste en un campo privado, la mejora (Rafaga 2 -> 1)
    // era INVISIBLE — la loc hardcodeaba "2". Como var, !RafagaCost! se renderiza y el up se ve.
    public int RafagaCost => DynamicVars["RafagaCost"].IntValue;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1), new DynamicVar("RafagaCost", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override bool IsPlayable => Rafaga.IsPlayable(Owner.Creature, RafagaCost);

    protected override bool ShouldGlowGoldInternal => Rafaga.ShouldGlow(Owner.Creature, RafagaCost);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Rafaga.Pay(choiceContext, Owner.Creature, RafagaCost, this);
        await PlayerCmd.GainEnergy(1, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["RafagaCost"].UpgradeValueBy(-1m); // RAFAGA 2 -> 1
}
