using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
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

    /// <summary>Marca de que el Aliento ya tocó 0 este turno (cap de 1 Tos/turno por agotamiento).</summary>
    public bool HitZeroThisTurn { get; set; }

    /// <summary>Tope actual = 10 base + lo que sumen los boosters de tope.</summary>
    public int Cap
    {
        get
        {
            var cap = Max;
            foreach (var p in Owner.GetPowerInstances<PowerModel>())
                if (p is IBreathCapBooster b) cap += b.ExtraBreathCap;
            if (Owner.Player != null)
                foreach (var r in Owner.Player.Relics)
                    if (r is IBreathCapBooster b) cap += b.ExtraBreathCap;
            return cap;
        }
    }

    /// <summary>Regen por turno = 2 base + lo que sumen los boosters de regen.</summary>
    public int Regen
    {
        get
        {
            var regen = RegenPerTurn;
            foreach (var p in Owner.GetPowerInstances<PowerModel>())
                if (p is IBreathRegenBooster b) regen += b.ExtraBreathRegen;
            return regen;
        }
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side) return;
        HitZeroThisTurn = false;
        var room = Math.Max(0, Cap - Amount);
        var gain = Math.Min(Regen, room);
        if (gain > 0)
        {
            Flash();
            await PowerCmd.ModifyAmount(this, gain, Owner, null);
        }
    }
}
