using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

/// <summary>
/// Artoria Avalon (Guardiana de la Espada Sagrada, A.A.) — permanent climax form
/// via Consagración de Avalon. Both passives, no Berserker penalty:
/// (a) first Skill each turn: +1 Star and NP +3; (b) Attacks +2 and CRITICAL enabled.
/// The NP payoff is the "Around Caliburn" window + the drafted NP cards (modelo 2026-06-12),
/// not an auto-generated ult card.
/// </summary>
public sealed class AvalonFormPower : ArtoriaFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/artoria_frames_avalon.tres";

    public override bool IsPermanent => true;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (FgoCombatState.GetTurn(Owner, 6) != 0 || cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Skill) return;

        await FgoCombatState.SetTurn(context, Owner, 6, 1, cardPlay.Card);
        Flash();
        await Stars.Gain(context, Owner, ProphecyCasterFormPower.StarsOnFirstSkill, null);
        await NpCharge.Gain(context, Owner, ProphecyCasterFormPower.NpOnFirstSkill, null);
    }

    public override decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer == Owner && props.IsPoweredAttack()) return SummerBerserkerFormPower.AttackBonus;
        return 0m;
    }
}
