using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Cards.Special;

/// <summary>
/// Metamorfosis de la Reina (女王的变身) — carta-token GRATIS (0⚡, Etérea + Exhaust) que el Cetro
/// de la Reina manifiesta en mano al empezar tu primer turno de cada combate (respuesta al feedback
/// de jugadores 2026-07-04: "¿puedo entrar a Caster desde el inicio o necesito draftear?" — Morgan
/// arrancaba SIN acceso a la forma sembradora hasta draftear un switch, y el early game de la Reina
/// detonaba un campo de Maldición casi vacío). Alterna Reina ↔ Bruja de la Lluvia; usarla cuenta como
/// cambio de forma, así que dispara el bono de primer-cambio del Cetro (+1⚡, robá 1, +10 NP) — el
/// turno 1 en Bruja es tempo-positivo. Etérea: si no la usás ese turno, se evapora (1 uso/combate).
/// En Invierno (forma clímax PERMANENTE) no es jugable: de la corona de invierno no se vuelve.
/// </summary>
public sealed class QueensMetamorphosis() : MorganCard(0, CardType.Skill, CardRarity.Event, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Ethereal];

    protected override bool IsPlayable =>
        Owner.Creature.HasPower<FairyQueenFormPower>() || Owner.Creature.HasPower<RainWitchFormPower>();

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<RainWitchFormPower>())
        {
            await FormSwitch.Enter<FairyQueenFormPower>(choiceContext, Owner.Creature, this);
        }
        else if (Owner.Creature.HasPower<FairyQueenFormPower>())
        {
            await FormSwitch.Enter<RainWitchFormPower>(choiceContext, Owner.Creature, this);
        }
    }
}
