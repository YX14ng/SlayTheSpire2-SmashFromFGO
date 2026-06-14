using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using OberonPretender.OberonPretenderCode.Cards;

namespace OberonPretender.OberonPretenderCode.Powers.Forms;

/// <summary>
/// EL REY DEL CUENTO (The Storybook King) -- forma INICIAL (modelo 2800100, alas de
/// mariposa radiantes). Pasiva (DESIGN-OBERON 5): La PRIMERA carta de Prestamo que jugas cada
/// turno te da +1 punto de Deuda -> +10 NP extra (el cuento endulza el trato).
///
/// El endulzante hace el prestamo mas barato AHORA a cambio de mas Deuda diferida: el +10 NP extra
/// se da en <see cref="AfterCardPlayed"/> y la carta-prestamo ya aplico su Deuda base; aca sumamos
/// el +1 punto del endulzante. Flag <see cref="_sweetenedThisTurn"/> = una sola vez por turno
/// (la primera). Reset al inicio de tu turno (heredando AfterSideTurnStart de FormPower).
/// </summary>
public sealed class StorybookKingPower : OberonFormPower
{
    public const int ExtraCharge = 10;
    public const int ExtraDebt = 1;

    public override string? FramesPath => $"{MainFile.ResPath}/character/oberon_frames_king.tres";

    private bool _sweetenedThisTurn;

    public override async Task AfterSideTurnStart(MegaCrit.Sts2.Core.Combat.CombatSide side, MegaCrit.Sts2.Core.Combat.CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == MegaCrit.Sts2.Core.Combat.CombatSide.Player) _sweetenedThisTurn = false;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_sweetenedThisTurn) return;
        if (cardPlay.Card is not ILoanCard || cardPlay.Card.Owner?.Creature != Owner) return;
        _sweetenedThisTurn = true;
        Flash();
        await NpCharge.Gain(Owner, ExtraCharge, null);
        await DebtPower.Add(Owner, ExtraDebt, Owner, null);
    }
}
