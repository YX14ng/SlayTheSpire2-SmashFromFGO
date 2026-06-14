using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Corte de las Hadas-Insecto (虫之妖精的宫廷 / Court of Insect-Fae) — DESIGN-OBERON §6.4. 2⚡ Poder: al
/// inicio de tu turno añadí una «Palabra del Rey Hada» a tu mano (cuesta su 1⚡ normal: el pool se cita a
/// sí mismo, sin regalar préstamos gratis). Aplica <see cref="CourtOfInsectFaePower"/>. El up baja el
/// coste a 1⚡.
/// </summary>
public sealed class CourtOfInsectFae() : OberonCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CourtOfInsectFaePower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
