using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using FGOCore.FGOCoreCode.Combat;
using OberonPretender.OberonPretenderCode.Cards.Basic;

namespace OberonPretender.OberonPretenderCode.Powers;

/// <summary>
/// Corte de las Hadas-Insecto (Court of Insect-Fae) -- DESIGN-OBERON 6.4. Al inicio de tu turno:
/// por cada <see cref="Amount"/> añadí una «Palabra del Rey Hada» a tu mano (cuesta su 1⚡ normal: el
/// pool se cita a sí mismo, con costo real para no regalar préstamos gratis). Counter: una 2a copia
/// genera +1. Manifestación vía el helper de FGOCore (CreateCard + AddGeneratedCardToCombat +
/// PreviewCardPileAdd), preview 0.8f.
/// </summary>
public sealed class CourtOfInsectFaePower : OberonPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await ManifestCards.ManifestToHand<KingFaesWord>(Owner, 0.8f);
        }
    }
}
