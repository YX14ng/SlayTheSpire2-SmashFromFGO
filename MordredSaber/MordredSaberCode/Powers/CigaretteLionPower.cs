using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// León del Cigarrillo B+ (香烟雄狮B+, §5.2) — el Rank-Up de Instinto B como PODER (guiño a Kairi
/// Sisigou). DESIGN-MORDRED §5.2: cada vez que OBTENÉS un *Crítico Listo* (FGOCore
/// <see cref="CritReadyPower"/> sube — típicamente el auto-proc de ★ a 100), robás 1 carta. El
/// +20★ inicial lo da la carta al jugarse (no el poder). Detección por el mismo hook que usa
/// CritStarsPower (<see cref="AfterPowerAmountChanged"/>): cuando el power que cambió es un
/// Crítico Listo del owner y subió (amount > 0), robamos. Counter: las copias apilan el robo.
/// El up de la carta baja el costo a 1⚡ (no toca el robo). Personal: no escala en multijugador.
/// </summary>
public sealed class CigaretteLionPower : MordredPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        await base.AfterPowerAmountChanged(power, amount, applier, cardSource);
        // Sólo nos importa que el owner GANE un Crítico Listo (amount > 0). Una copia roba 1 por
        // cada crítico obtenido; varias copias multiplican el robo (Counter).
        if (power is not CritReadyPower || power.Owner != Owner || amount <= 0) return;
        if (Owner.Player == null || Owner.IsDead) return;
        Flash();
        await CardPileCmd.Draw(new BlockingPlayerChoiceContext(), (int)Amount, Owner.Player);
    }
}
