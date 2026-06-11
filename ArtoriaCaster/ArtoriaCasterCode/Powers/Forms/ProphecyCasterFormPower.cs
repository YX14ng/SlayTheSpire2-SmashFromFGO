using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

/// <summary>
/// Niña de la Profecía (Caster, 預言の子) — Castoria's starting form.
/// The first Skill you play each turn: gain 1 Critical Star and NP Charge +3.
/// No numeric penalty: the opportunity cost is built in (attacks can't crit here).
/// </summary>
public sealed class ProphecyCasterFormPower : ArtoriaFormPower
{
    public const int StarsOnFirstSkill = 1;
    public const int NpOnFirstSkill = 3;

    public override string FramesPath => $"{MainFile.ResPath}/character/artoria_frames_caster.tres";

    private bool _firedThisTurn;

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        await base.AfterSideTurnStart(side, combatState);
        if (side == CombatSide.Player)
        {
            _firedThisTurn = false;
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_firedThisTurn || cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;

        _firedThisTurn = true;
        Flash();
        await Stars.Gain(Owner, StarsOnFirstSkill, null);
        await NpCharge.Gain(Owner, NpOnFirstSkill, null);
    }
}
