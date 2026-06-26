using MegaCrit.Sts2.Core.Commands;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>
/// Munición Conceptual — Power. Re-perfilado P2 2026-06-25: era un generador que casi nunca
/// procaba (1⚡→2⚡, quitaba 1 buff/turno antes de cada Ataque, en conflicto con las cartas que
/// ya stripean). Ahora aplica <see cref="ConceptualAmmoPower"/>: cada vez que quitás un buff
/// enemigo con cualquier carta, ganás 6 Estrellas de Crítico (up: 9). Bajado a 1⚡ — es el
/// payoff pasivo del eje de strip, no un motor activo.
/// </summary>
public sealed class ConceptualAmmo() : MashShielderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ConceptualAmmoPower>("ConceptualAmmo", 6m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ConceptualAmmoPower>(choiceContext, Owner.Creature, DynamicVars["ConceptualAmmo"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ConceptualAmmo"].UpgradeValueBy(3m);
    }
}
