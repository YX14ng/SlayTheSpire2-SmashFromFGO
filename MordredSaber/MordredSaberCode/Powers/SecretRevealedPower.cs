using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Powers.Forms;
using FormsHelper = MordredSaber.MordredSaberCode.Powers.Forms.Forms;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Secreto Revelado (秘密揭露, §5.3) — el motor de la danza: cada vez que TE QUITÁS EL YELMO (entrás
/// en Rebelión, un cambio de forma iniciado por el jugador), ganás +<see cref="Stars"/> Estrellas y
/// robás <see cref="Cards"/>. Implementa <see cref="IFormChangeListener"/> (lo notifica FormSwitch.Enter,
/// igual que el Corazón de Homúnculo de Mash) y solo proca al pasar a Rebelión (la revelación), no en
/// cualquier swap. El <see cref="Stars"/> es campo settable que fija la carta; Amount = conteo de
/// stacks (Counter, las copias suman las ★). Personal.
/// </summary>
public sealed class SecretRevealedPower : MordredPower, IFormChangeListener
{
    public int Stars = 20;
    public int Cards = 1;

    /// <summary>
    /// REDESIGN-MORDRED-V2 D5 / Candado 2 — tope de activaciones por turno. El disparador es un
    /// CAMBIO DE FORMA, que el jugador controla y puede repetir cuantas veces quiera en un turno con
    /// cartas de 0⚡ (Rugido de Rebelión, Visita Relámpago): sin tope esto es generación gratuita
    /// ilimitada, no un poder por turno. Idioma del propio personaje: `AccumulatedHatredPower`
    /// (MaxProcsPerTurn = 2) y la starter (3 conversiones/turno). Bits 12-13 del campo de
    /// turno de FGOCore (se resetea solo al empezar tu turno).
    /// </summary>
    public int MaxProcsPerTurn = 2;


    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebellionFormPower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        // Solo cuenta la REVELACIÓN: arrancarse el yelmo (entrar en Rebelión).
        if (!FormsHelper.InRebellion(Owner)) return;
        var context = choiceContext ?? new BlockingPlayerChoiceContext();
        if (FgoCombatState.GetTurn(Owner, 12, 2) >= MaxProcsPerTurn) return;
        await FgoCombatState.IncrementTurn(context, Owner, 12, MaxProcsPerTurn, null, width: 2);
        Flash();
        await CritStars.Gain(context, Owner, Stars * (int)Amount, null);
        var player = Owner.Player;
        if (choiceContext != null && player?.PlayerCombatState is { } playerCombatState)
        {
            // BUGFIX (soft-lock): el cambio de forma lo dispara una carta a MITAD de su resolución.
            // Si este robo RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
            // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
            // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
            var inDeck = playerCombatState.AllPiles
                .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
            var toDraw = System.Math.Min(Cards, inDeck);
            if (toDraw > 0)
            {
                await CardPileCmd.Draw(choiceContext, toDraw, player);
            }
        }
    }
}
