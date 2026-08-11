using BaseLib.Abstracts;
using FGOCore.FGOCoreCode.CardTypes;
using FGOCore.FGOCoreCode.Np;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode.Bond;

/// <summary>
/// 好感度 (Bond) — FGO-style per-run bond system. The character earns Bond points from
/// combat victories (+2, elite +3, boss +5) and from entering events, shops and rest
/// sites (+1). Crossing a level threshold grants permanent gifts; defaults:
///   Lv 1/3/6/9  → +3/+3/+4/+5 Max HP (immediately)
///   Lv 2/5/8    → start combats with 5/10/15 NP Charge
///   Lv 4/7      → start combats with 3/6 Block
///   Lv 10       → character capstone via <see cref="ApplyCapstone"/>
/// Subclass per character (the subclass owns the relic ID, art and localization, and
/// may override any curve). The relic counter shows the current Bond level; Points
/// persist through save/continue via SavedProperty.
/// In multiplayer, Max HP and Block gifts scale x(1 + 0.5*(players-1)) — monsters get
/// x(count*1.1-1.3) HP and a tank absorbs the whole team's damage. NP stays flat.
/// </summary>
public abstract class BondRelic : CustomRelicModel
{
    private int _points;

    // 2026-06-12: ELIMINADO el multiplicador global de daño/bloqueo (×1.4) y la regen
    // por turno (3) que se habían metido para los jefes del mod JeanneAlter. El usuario
    // sacó ese mod, y el análisis de 15 mods del ecosistema confirmó que NINGUNO usa un
    // ×daño global desde el starter — era la causa raíz de "demasiado rotos" (skill §1.bis
    // regla 2). El Bond vuelve a ser SOLO regalos por umbral (HP/NP/Bloqueo + capstone),
    // estilo Mordekaiser soulcrown. La potencia del personaje viene de sus motores
    // gateados (Carga NP, Formas, Estrellas, Baluarte), no de una tasa plana.

    public override bool ShowCounter => true;

    public override int DisplayAmount => Level;

    protected virtual int[] Thresholds => [5, 12, 20, 30, 40, 52, 64, 76, 88, 100];

    [SavedProperty]
    public int Points
    {
        get => _points;
        set
        {
            AssertMutable();
            _points = value;
            InvokeDisplayAmountChanged();
        }
    }

    /// <summary>Points between each bond level beyond the base cap (Holy Grail territory).</summary>
    protected virtual int ExtraThresholdStep => 14;

    /// <summary>Max HP gift for each level beyond the base cap.</summary>
    protected virtual decimal MaxHpGainBeyondCap => 5m;

    /// <summary>Extra levels unlocked by owned <see cref="ILimitBreaker"/> relics.</summary>
    protected int ExtraLevels
    {
        get
        {
            var extra = 0;
            foreach (var relic in Owner.Relics)
            {
                if (relic is ILimitBreaker breaker) extra += breaker.ExtraBondLevels;
            }
            return extra;
        }
    }

    public int Level
    {
        get
        {
            var lv = 0;
            foreach (var t in Thresholds)
            {
                if (Points >= t) lv++;
            }
            if (lv >= Thresholds.Length)
            {
                var t = Thresholds[^1];
                for (var i = 0; i < ExtraLevels; i++)
                {
                    t += ExtraThresholdStep;
                    if (Points >= t) lv++;
                }
            }
            return lv;
        }
    }

    protected virtual int StartingNp(int lv) => lv >= 8 ? 15 : lv >= 5 ? 10 : lv >= 2 ? 5 : 0;

    protected virtual int StartingBlock(int lv) => lv >= 7 ? 6 : lv >= 4 ? 3 : 0;

    protected virtual decimal MaxHpGainAt(int lv) => lv switch { 1 => 3m, 3 => 3m, 6 => 4m, 9 => 5m, _ => 0m };

    /// <summary>Lv 10 reward, applied at the start of every combat. Default: nothing.</summary>
    protected virtual Task ApplyCapstone() => Task.CompletedTask;

    protected virtual int PointsForVictory(RoomType roomType) => roomType switch
    {
        RoomType.Boss => 5,
        RoomType.Elite => 3,
        _ => 2
    };

    protected decimal MultiplayerFactor => 1m + (Owner.RunState.Players.Count - 1) * 0.5m;

    public override async Task AfterCombatVictory(CombatRoom room)
    {
        await AddPoints(PointsForVictory(room.RoomType));
    }

    // Punto de mapa (acto + col/row) del último +1 por sala; 0 = ninguno. GUARDADO a propósito: el
    // motor re-dispara AfterRoomEntered al continuar un save — EventRoom.EnterInternal llama
    // Hook.AfterRoomEntered incondicionalmente (ni IsPreFinished ni isRestoringRoomStackBase lo
    // saltean) y, si el save quedó dentro del combate interno de un evento, RunManager re-entra
    // además el EventRoom padre por ParentEventId. Como hay saves que se escriben DESPUÉS del +1
    // (EventRoom.OnEventStateChanged al terminar un Ancient, y la victoria de CombatManager), sin
    // esta clave cada ciclo salir/continuar sumaba otro punto permanente. Se usa el punto de mapa y
    // no AbstractRoom.Id porque el motor documenta ese Id como NO estable a través de save/load,
    // mientras que VisitedMapCoords sí se persiste y se restaura antes de entrar a la sala.
    private int _lastBondMapPoint;

    [SavedProperty]
    protected int LastBondMapPoint
    {
        get => _lastBondMapPoint;
        set
        {
            AssertMutable();
            _lastBondMapPoint = value;
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is not (RoomType.Event or RoomType.Shop or RoomType.RestSite)) return;

        // Sin coordenada no hay marca idempotente posible: salimos en vez de premiar y pisar la
        // clave anterior con un 0 que ya no puede coincidir con nada (fallaría abierto y encima
        // olvidaría la última sala legítima). En el flujo vanilla la ventana sin coordenada solo
        // contiene la MapRoom, que ni siquiera está en los tipos premiados.
        var mapPoint = CurrentMapPointKey();
        if (mapPoint == 0 || mapPoint == LastBondMapPoint) return;

        LastBondMapPoint = mapPoint;
        await AddPoints(1);
    }

    /// <summary>Clave estable del punto de mapa actual, o 0 si todavía no hay coordenada. Cada
    /// componente va +1 para que 0 nunca sea una posición válida.</summary>
    private int CurrentMapPointKey()
    {
        var location = Owner.RunState.MapLocation;
        if (location.coord is not { } coord) return 0;
        return (location.actIndex + 1) * 1_000_000 + (coord.col + 1) * 1_000 + (coord.row + 1);
    }

    // Último nivel ya premiado con regalos de subida. TRANSIENT a propósito (no SavedProperty): se
    // inicializa perezosamente al Level actual (al cargar un save, los niveles previos ya cobraron su
    // Max HP, que está guardado), así un update del mod no re-regala niveles viejos. El premio corre
    // por GrantPendingLevelGifts, que también se invoca en BeforeCombatStartLate — eso cubre el salto
    // de Level al obtener un ILimitBreaker (Santo Grial) con puntos ya bancados más allá del tope
    // (audit 2026-07-04: esos regalos se perdían porque solo AddPoints premiaba, mirando el delta).
    private int? _rewardedLevel;

    protected async Task AddPoints(int pts)
    {
        _rewardedLevel ??= Level; // antes de sumar: lo anterior ya fue premiado
        Points += pts;
        await GrantPendingLevelGifts();
    }

    /// <summary>Punto ÚNICO de premiación: concede los regalos de cada nivel alcanzado y aún no
    /// premiado (Level &gt; nivel premiado). Idempotente; seguro de llamar en cada combate.</summary>
    protected async Task GrantPendingLevelGifts()
    {
        _rewardedLevel ??= Level;
        var lv = Level;
        if (lv <= _rewardedLevel.Value) return;

        Flash();
        var from = _rewardedLevel.Value;
        _rewardedLevel = lv;
        await GrantLevelUpGifts(from, lv);
    }

    /// <summary>
    /// Concede los regalos de cruzar de nivel (Max HP por umbral) para cada nivel en
    /// (<paramref name="fromExclusive"/>, <paramref name="toInclusive"/>]. Helper protegido
    /// NUEVO (2026-06-15) para que el bucle de la curva viva en un solo lugar: la base lo llama
    /// desde <see cref="AddPoints"/>; las subclases siguen ajustando SÓLO la curva vía
    /// <see cref="MaxHpGainAt"/> / <see cref="MaxHpGainBeyondCap"/> (API sin cambios). ADITIVO.
    /// </summary>
    protected async Task GrantLevelUpGifts(int fromExclusive, int toInclusive)
    {
        for (var lv = fromExclusive + 1; lv <= toInclusive; lv++)
        {
            var hp = MaxHpGiftAt(lv);
            if (hp > 0)
            {
                await CreatureCmd.GainMaxHp(Owner.Creature, Math.Round(hp * MultiplayerFactor));
            }
        }
    }

    /// <summary>Max HP del regalo de nivel <paramref name="lv"/>, eligiendo entre la curva base
    /// (<see cref="MaxHpGainAt"/>, dentro del tope) y la de Santo Grial (<see cref="MaxHpGainBeyondCap"/>,
    /// más allá del tope). Punto único que decide qué curva aplica. ADITIVO.</summary>
    protected decimal MaxHpGiftAt(int lv) => lv <= Thresholds.Length ? MaxHpGainAt(lv) : MaxHpGainBeyondCap;

    public override async Task BeforeCombatStartLate()
    {
        // Motor de TIPOS DE CARTA (Buster/Arts/Quick): todos los Servants reciben el power que reparte
        // el bonus al jugar cartas que implementan ICommandTyped. Independiente del nivel de Bond — va
        // SIEMPRE, antes del early-return de los regalos por nivel. ADITIVO (2026-06-26, Tarea C).
        await CommandBonusPower.EnsureInstalled(Owner.Creature);

        // Cobrar regalos pendientes de un salto de nivel fuera de AddPoints (Santo Grial obtenido
        // con puntos ya acumulados más allá del tope) — ver _rewardedLevel.
        await GrantPendingLevelGifts();

        var lv = Level;
        if (lv <= 0) return;

        await ApplyCombatStartGifts(lv);
    }

    /// <summary>
    /// Aplica los regalos de "al empezar el combate" para el nivel <paramref name="lv"/>:
    /// <see cref="BondPower"/> = nivel, Carga NP inicial (<see cref="StartingNp"/>), Bloqueo
    /// inicial (<see cref="StartingBlock"/>) y, a nivel 10, <see cref="ApplyCapstone"/>. Helper
    /// protegido NUEVO (2026-06-15): la base lo llama desde <see cref="BeforeCombatStartLate"/>;
    /// las subclases siguen ajustando SÓLO los hooks de la curva (API sin cambios). ADITIVO.
    /// </summary>
    protected async Task ApplyCombatStartGifts(int lv)
    {
        await PowerCmd.Apply<BondPower>(new BlockingPlayerChoiceContext(), Owner.Creature, lv, Owner.Creature, null, silent: true);

        var np = StartingNp(lv);
        if (np > 0)
        {
            await NpCharge.Gain(Owner.Creature, np, null);
        }

        var block = StartingBlock(lv);
        if (block > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, Math.Round(block * MultiplayerFactor), ValueProp.Unpowered, null);
        }

        if (lv >= 10)
        {
            await ApplyCapstone();
        }
    }
}
