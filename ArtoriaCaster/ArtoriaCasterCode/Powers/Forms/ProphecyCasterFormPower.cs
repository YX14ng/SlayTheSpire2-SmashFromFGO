using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

/// <summary>
/// Niña de la Profecía (Caster, 預言の子) — Castoria's starting form.
/// The first Skill you play each turn: gain 1 Critical Star and NP Charge +3.
/// No numeric penalty: the opportunity cost is built in (attacks can't crit here).
/// </summary>
public sealed class ProphecyCasterFormPower : ArtoriaFormPower
{
    public const int StarsOnFirstSkill = 10;
    // 3 -> 5 en el re-baseo al entorno Hextech+BetterCharacters (DESIGN-ARTORIA §8.bis).
    public const int NpOnFirstSkill = 5;

    public override string FramesPath => $"{MainFile.ResPath}/character/artoria_frames_caster.tres";

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (FgoCombatState.GetTurn(Owner, 6) != 0 || cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;

        await FgoCombatState.SetTurn(context, Owner, 6, 1, cardPlay.Card);
        Flash();
        await Stars.Gain(context, Owner, StarsOnFirstSkill, null);
        await NpCharge.Gain(context, Owner, NpOnFirstSkill, null);
    }
}
