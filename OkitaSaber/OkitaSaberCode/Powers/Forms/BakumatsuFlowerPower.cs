using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Cards;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Powers.Forms;

/// <summary>
/// Flor del Bakumatsu (幕末之华) — la ÚNICA forma de Okita, el clímax PERMANENTE (DESIGN-OKITA §3.4):
/// la enfermedad ya ganó, pelea igual. (1) Tus *RÁFAGAS dejan de costar *Aliento (<see cref="IRafagaCostModifier"/>,
/// sin pago de HP). (2) Al final de cada turno ganás 1 *Tos al mazo de robo. Hace el swap de modelo
/// 102710 (traje blanco) → 102720 (haori asagi) vía <see cref="FormVisuals"/> (precarga en hilos).
///
/// <see cref="FormPower.IsPermanent"/> = true: una vez entrás, nunca se reemplaza (FormSwitch lo respeta).
/// El power lo aplica la carta «Flor del Bakumatsu» (Poder 2⚡, Exhaust) vía FormSwitch.Enter.
/// </summary>
public sealed class BakumatsuFlowerPower : OkitaFormPower, IRafagaCostModifier
{
    public override bool IsPermanent => true;

    // Modelo haori asagi (102720). Si los frames aún no existen, FormVisuals loguea y mantiene el sprite actual.
    public override string? FramesPath => $"{MainFile.ResPath}/character/okita_frames_haori.tres";

    public bool WaivesBreathCost => true;

    public int HpPerBreathPoint => 0; // gratis, sin pagar HP (a diferencia de «Hasta el Final»)

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return;
        Flash();
        await Tos.ShuffleIntoDraw(Owner, null);
    }
}
