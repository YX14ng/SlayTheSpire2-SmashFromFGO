using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Algo (un power o una reliquia del jugador) que reacciona cuando se CONSUME un *Crítico
/// Listo* (FGOCore <c>CritReadyPower</c> decrementa). DESIGN-MORDRED §4.1: el motor del
/// personaje (★→×2→NP). Como FGOCore no expone un hook al decrementar el crítico (y esta
/// fase NO toca FGOCore), la detección es 100% mod-local: el watcher
/// <see cref="RedLightningChannelPower"/> (aplicado al iniciar combate por la starter)
/// compara la cuenta de Crítico Listo tras cada carta y, al ver una baja, avisa a TODOS
/// los listeners (powers del owner + reliquias del jugador) — mismo patrón de broadcast
/// que <c>FormSwitch.Enter</c> usa para <c>IFormChangeListener</c>.
///
/// Lo consumen: la starter «Clarent, la Espada Robada» (+10 NP por crítico), y en la fase
/// Content «Insolente», «La Espada Más Resplandeciente», «Aceleración de Homúnculo», los
/// riders «si consumiste un CRÍTICO este turno», etc.
/// </summary>
public interface ICritConsumedListener
{
    Task OnCritConsumed(PlayerChoiceContext? choiceContext);
}
