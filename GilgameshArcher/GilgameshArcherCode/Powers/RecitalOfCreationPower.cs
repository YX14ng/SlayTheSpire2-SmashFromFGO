using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using FGOCore.FGOCoreCode.Stars;

namespace GilgameshArcher.GilgameshArcherCode.Powers;

/// <summary>
/// Recital de la Creación (开辟之咏唱) — DESIGN-GILGAMESH §5.4. Cose NP→crítico en el clímax: cada vez
/// que se manifiesta tu Enuma Elish (cruzás 100 de Carga NP) ganás +1 *Crítico Listo por stack y robás 1.
///
/// Auto-contenido (NO toca MainFile): observa <see cref="AfterPowerAmountChanged"/> buscando que el
/// <see cref="EnumaManifestedPower"/> del dueño suba (amount &gt; 0). Ese marcador lo aplica MainFile UNA
/// vez por pico vía <c>NpCharge.GaugeFilled</c> (y lo retira al caer bajo 100), así que el límite
/// «1 proc por pico» sale gratis del marcador ya cableado. Patrón CombatAnalysisPower /
/// MakotoBanner (AfterPowerAmountChanged + BlockingPlayerChoiceContext para el robo). Buff visible,
/// Counter (el up es −1⚡ de coste, no más stacks), personal (no escala en MP).
/// </summary>
public sealed class RecitalOfCreationPower : GilgameshPower
{
    public const int DrawPerManifest = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritReadyPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        if (amount <= 0m || power is not EnumaManifestedPower || power.Owner != Owner || Owner.Player == null) return;

        Flash();
        await PowerCmd.Apply<CritReadyPower>(Owner, Amount, Owner, null);
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), DrawPerManifest, Owner.Player);
    }
}
