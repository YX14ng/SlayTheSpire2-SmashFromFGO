using System.Collections.Generic;
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
public sealed class SeaOfLifeWomb : TiamatRelic
{
    public const int StartingNp = 10;
    public const int LahmuOnCombatStart = 1;
    public const int LahmuPerFirstCurse = 1;

    public override RelicRarity Rarity => RelicRarity.Starter;

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
        _firstCursedEnemies.Clear();
        Flash();
        await FormSwitch.Enter<TiamatFemmeFatalePower>(null, Owner.Creature, null);
        await NpCharge.Gain(Owner.Creature, StartingNp, null);
        await Lahmu.Spawn(Owner.Creature, LahmuOnCombatStart, null);
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
        await Lahmu.Spawn(Owner.Creature, LahmuPerFirstCurse, null);
    }
}
