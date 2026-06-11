using BaseLib.Abstracts;
using FGOCore.FGOCoreCode.Np;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
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

    /// <summary>
    /// Servant Ascendido: subida de tasa global para el entorno modded real del
    /// usuario (HextechRunes endurece a los enemigos con hexes ×1.2-1.5 y los
    /// personajes vanilla llevan BetterCharacters; a tasa vanilla pura nuestros
    /// servants quedan ~25-50% por debajo). Es la misma palanca multiplicativa
    /// que usan las runas Hextech. Perilla de playtest: 1.25 → 1.4 si sigue floja.
    /// </summary>
    public virtual decimal ServantDamageMultiplier => 1.25m;

    public virtual decimal ServantBlockMultiplier => 1.25m;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner.Creature || !props.IsPoweredAttack()) return 1m;
        return ServantDamageMultiplier;
    }

    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, MegaCrit.Sts2.Core.Entities.Cards.CardPlay? cardPlay)
    {
        if (target != Owner.Creature) return 1m;
        return ServantBlockMultiplier;
    }

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

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.RoomType is RoomType.Event or RoomType.Shop or RoomType.RestSite)
        {
            await AddPoints(1);
        }
    }

    protected async Task AddPoints(int pts)
    {
        var before = Level;
        Points += pts;
        var after = Level;
        if (after <= before) return;

        Flash();
        for (var lv = before + 1; lv <= after; lv++)
        {
            var hp = lv <= Thresholds.Length ? MaxHpGainAt(lv) : MaxHpGainBeyondCap;
            if (hp > 0)
            {
                await CreatureCmd.GainMaxHp(Owner.Creature, Math.Round(hp * MultiplayerFactor));
            }
        }
    }

    public override async Task BeforeCombatStartLate()
    {
        var lv = Level;
        if (lv <= 0) return;

        await PowerCmd.Apply<BondPower>(Owner.Creature, lv, Owner.Creature, null, silent: true);

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
