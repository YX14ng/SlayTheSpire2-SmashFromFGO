using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>Marker: este power/reliquia sube el TOPE del *Aliento (Dango de Tres Colores,
/// Respiración del Tennen Rishin-ryū). AlientoPower lo suma para su Cap.</summary>
public interface IBreathCapBooster
{
    int ExtraBreathCap { get; }
}

/// <summary>Marker: este power/reliquia cambia el REGEN de *Aliento por turno (en vez de +2).
/// Respiración del Tennen Rishin-ryū lo lleva a 3.</summary>
public interface IBreathRegenBooster
{
    int ExtraBreathRegen { get; }
}

/// <summary>
/// Aliento (吐息 / Breath) — el EMBUDO de Okita (DESIGN-OKITA §3). Contador 0-<see cref="Max"/>;
/// empieza cada combate en <see cref="StartingBreath"/> (lo fija el Haori) y recupera 2 (base) al
/// inicio de tu turno. Lo paga la keyword *RÁFAGA (1-3 puntos) además del ⚡ — la doble moneda
/// (precedente: ⚡+★ del Regent). 1 Aliento ≈ ½⚡: income 2/turno + banco 10 → subsidio máx ~1⚡/turno.
///
/// El tope (<see cref="Cap"/>) y el regen (<see cref="Regen"/>) los engordan los boosters
/// (<see cref="IBreathCapBooster"/> / <see cref="IBreathRegenBooster"/>: Dango, Tennen Rishin-ryū),
/// sumados dinámicamente cada turno — sin mutar campos desde varias fuentes. Si el Aliento llega a 0
/// por una Ráfaga, ganás 1 *Tos (máx. 1/turno, ver <see cref="Aliento"/>).
///
/// Counter, personal: no escala en multijugador.
/// </summary>
public sealed class AlientoPower : OkitaPower
{
    public const int Max = 10;
    public const int StartingBreath = 6;
    public const int RegenPerTurn = 2;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Tope actual = 10 base + lo que sumen los boosters de tope (powers + reliquias).</summary>
    public int Cap
    {
        get
        {
            var cap = Max;
            Listeners.ForEach<IBreathCapBooster>(Owner, b => cap += b.ExtraBreathCap);
            return cap;
        }
    }

    /// <summary>Regen por turno = 2 base + lo que sumen los boosters de regen (solo powers).</summary>
    public int Regen
    {
        get
        {
            var regen = RegenPerTurn;
            foreach (var b in Listeners.PowersOf<IBreathRegenBooster>(Owner))
                regen += b.ExtraBreathRegen;
            return regen;
        }
    }

    // El REGEN por turno ya no vive aca (audit 2026-07-04): este power se REMUEVE al llegar a 0 y el
    // regen moria con el para el resto del combate. Ahora regenera el Haori Asagi (starter, siempre
    // presente) via Aliento.Gain, que reinstala este power si falta. El flag HitZeroThisTurn tambien
    // migro a Aliento (estatico per-creature) por el mismo motivo.
}
