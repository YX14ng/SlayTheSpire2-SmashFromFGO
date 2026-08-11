using System.Collections.Generic;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using TiamatBeast.TiamatCode.Powers.Forms;

namespace TiamatBeast.TiamatCode.Relics;

/// <summary>
/// Útero del Mar de Vida — reliquia STARTER (el motor de Tiamat, REDESIGN-TIAMAT §74).
/// Al iniciar cada combate: entrás en la forma Femme Fatale (la criadora), ganás
/// <see cref="StartingNp"/> de Carga NP y parís <see cref="LahmuOnCombatStart"/> Laḫmu.
/// Además, la 1ª vez que CADA enemigo se cursa, parís 1 Laḫmu (la Maldición→cría literal:
/// ata el puente Maldición↔enjambre desde el turno 1).
///
/// El gancho "1ª maldición por enemigo" NO necesita API nueva en FGOCore (esta fase no lo
/// toca): leemos <see cref="AfterPowerAmountChanged"/> — disparado globalmente por PowerCmd
/// para CUALQUIER cambio de power — y reaccionamos cuando un <c>CursePower</c> sube (amount &gt; 0)
/// sobre un enemigo nunca antes contado este combate (patrón MakotoBanner/KairisCigarettes).
/// (source=null en el FormSwitch inicial: fija la forma sin contar como "cambio de forma".)
/// </summary>
public class SeaOfLifeWomb : TiamatRelic, INpLevelStore
{
    // ---- Gacha de dupes / INpLevelStore (audit 2026-07-05, HIGH) ----
    // Sin un INpLevelStore el medidor de NP capeaba en 100 PARA SIEMPRE (NpCharge.Max lee el nivel
    // del store del jugador): la decision central "abrir a 100 vs banquear a 300" era inalcanzable
    // y el escalado NpLevels.Scale de las cartas NP quedaba clavado en x1. Patron calcado de
    // SummonTicket (Mash) / MorganSummonSeal, incluido el gate >=3 que convive con Driftwood.
    public const string DupeOptionId = "TIAMAT_DUPE";

    private int _npLevel = 1;
    private int _dupePity;

    public override bool ShowCounter => true;

    public override int DisplayAmount => NpLevel;

    [SavedProperty]
    public int NpLevel
    {
        get => _npLevel;
        set
        {
            AssertMutable();
            _npLevel = value;
            InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty]
    public int DupePity
    {
        get => _dupePity;
        set
        {
            AssertMutable();
            _dupePity = value;
        }
    }

    public override bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)
    {
        return NpDupeAlternative.TryAdd(
            Owner, player, alternatives, DupeOptionId, OnDupeRoll);
    }

    private async Task OnDupeRoll()
    {
        if (await NpLevels.TryRollDupeWithConsolation(Owner))
        {
            Flash();
        }
    }

    public const int StartingNp = 10;
    public const int LahmuOnCombatStart = 1;
    public const int LahmuPerFirstCurse = 1;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<SeaOfLifeGenesis>();

    protected virtual int StartingNpAmount => StartingNp;
    protected virtual int StartingLahmuAmount => LahmuOnCombatStart;
    protected virtual int LahmuPerFirstCurseAmount => LahmuPerFirstCurse;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<LahmuSwarmPower>(),
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CursePower>(),
    ];

    // Enemigos cuya 1ª maldición ya disparó el parto este combate (no re-disparar si la
    // Maldición decae a 0 y se vuelve a sembrar). Se reinicia en cada BeforeCombatStartLate.
    private readonly HashSet<Creature> _firstCursedEnemies = [];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await CommandBonusPower.EnsureInstalled(Owner.Creature);
        _firstCursedEnemies.Clear();
        Flash();
        await FormSwitch.Enter<TiamatFemmeFatalePower>(null, Owner.Creature, null);
        await NpCharge.Gain(Owner.Creature, StartingNpAmount, null);
        await Lahmu.Spawn(Owner.Creature, StartingLahmuAmount, null);
    }

    // amount > 0 sobre un CursePower de un ENEMIGO = ese enemigo recibió Maldición. Si es la 1ª
    // vez que lo registramos este combate, parí 1 Laḫmu (Maldición→cría). Una vez por enemigo.
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0m || power is not CursePower) return;
        var enemy = power.Owner;
        if (enemy == null || !enemy.IsMonster) return;
        if (!_firstCursedEnemies.Add(enemy)) return;
        Flash();
        await Lahmu.Spawn(choiceContext, Owner.Creature, LahmuPerFirstCurseAmount, null);
    }
}

/// <summary>
/// Mar de Vida: Génesis. La evolución Ancient del Útero conserva la forma, el motor de
/// Maldición→Laḫmu y el nivel de NP, pero abre cada combate con el doble de NP y de cría.
/// </summary>
public sealed class SeaOfLifeGenesis : SeaOfLifeWomb
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override int StartingNpAmount => 20;
    protected override int StartingLahmuAmount => 2;

}
