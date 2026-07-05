using System.Linq;
using FGOCore.FGOCoreCode.Forms;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using TiamatBeast.TiamatCode.Cards;
using TiamatBeast.TiamatCode.Powers.Forms;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// La VENTANA Bestia (rediseño dos-pozas, ver docs/REDESIGN-TIAMAT.md). Counter = duración en
/// turnos (1-3, según el tier de Sobrecarga que se consumió al abrir). Mientras está activa,
/// Tiamat es Bestia II y tiene en mano el mazo especial efímero (<see cref="IBeastEphemeral"/>).
/// Decrementa al inicio de cada uno de TUS turnos (el turno de apertura no cuenta — ese hook ya
/// pasó); al llegar a 0 cierra: REVIERTE a Lily (Femme Fatale) y PURGA las cartas efímeras que
/// queden — nunca contaminan el mazo persistente.
/// </summary>
public sealed class TiamatBeastWindowPower : TiamatPower
{
    private bool _closed;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        Flash();
        await PowerCmd.Decrement(this);
        if (Amount <= 0)
        {
            await CloseWindow(choiceContext);
        }
    }

    /// <summary>Cierra la ventana: revierte a Lily, purga el mazo efímero y se remueve. Idempotente
    /// (re-entrante seguro). Llamable también desde una carta de cierre o el fin de combate.</summary>
    public async Task CloseWindow(PlayerChoiceContext? choiceContext = null)
    {
        if (_closed) return;
        _closed = true;

        if (Owner.HasPower<TiamatBeastPower>())
        {
            // Propagar el contexto sincronizado del caller (audit 2026-07-05, regla 4 del proyecto);
            // el fallback fresco queda solo para call-sites sin contexto (fin de combate).
            await FormSwitch.Enter<TiamatFemmeFatalePower>(choiceContext, Owner, null);
        }
        await PurgeEphemeral(Owner.Player);
        // Decrement a 0 ya auto-remueve el power: remover de nuevo disparaba AfterRemoved dos veces.
        if (Owner.GetPower<TiamatBeastWindowPower>() == this)
        {
            await PowerCmd.Remove(this);
        }
        // Re-chequear el Genesis (audit 2026-07-05, HIGH): si la ventana expiro con el medidor >=100,
        // GaugeFilled no vuelve a disparar (no hay cruce) y sin esto el medidor quedaba brickeado.
        await MainFile.EnsureGenesisInHand(Owner);
    }

    /// <summary>Quita de combate TODA carta <see cref="IBeastEphemeral"/> en mano/robo/descarte:
    /// si no se jugaron en la ventana, se esfuman (no entran al mazo Lily persistente). Estática
    /// para poder llamarla también desde el fin de combate.</summary>
    public static async Task PurgeEphemeral(Player? player)
    {
        if (player == null) return;
        var ephemeral = CardPile.GetCards(player, PileType.Hand, PileType.Draw, PileType.Discard)
            .Where(c => c is IBeastEphemeral)
            .ToList();
        if (ephemeral.Count > 0)
        {
            await CardPileCmd.RemoveFromCombat(ephemeral);
        }
    }
}
