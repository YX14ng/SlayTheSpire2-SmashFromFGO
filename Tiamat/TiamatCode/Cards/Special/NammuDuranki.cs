using FGOCore.FGOCoreCode.CardTypes;
using FGOCore.FGOCoreCode.Cleanse;
using FGOCore.FGOCoreCode.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Cards;
using TiamatBeast.TiamatCode.Powers;
using TiamatBeast.TiamatCode.Powers.Forms;
using TiamatBeast.TiamatCode.Powers.Seal;

namespace TiamatBeast.TiamatCode.Cards.Special;

/// <summary>
/// Nammu Dur-an-ki (ナンム・ドゥルアンキ) — la carta-NP de APERTURA de la ventana Bestia (rediseño
/// dos-pozas, ver docs/REDESIGN-TIAMAT.md). Se MANIFIESTA en la mano cuando el medidor cruza 100
/// (no es gratis: el jugador decide CUÁNDO jugarla — a 100 una ventana corta, o banqueada a 300 una
/// larga). NO es <see cref="IBeastEphemeral"/>: es la llave de la ventana, no parte del mazo Bestia.
///
/// Al jugarse: consume TODA la Carga NP (tier = lo consumido, ≥100); la DURACIÓN de la ventana =
/// clamp(tier/100, 1, 3) turnos. Limpia tus debuffs; pega un AoE <b>FIJO</b> (14 + tier/10) a todos
/// y los SELLA (Sello de Habilidad); te volvés Bestia II; manifiesta el mazo Bestia entero; abre la
/// ventana (Counter=duración) y devuelve recursos (+1⚡, robar 2). El AoE es fijo a propósito (no
/// ConsumeAll) para NO apilarse con el cierre (Pluma de la Bestia).
/// </summary>
public sealed class NammuDuranki() : TiamatCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Genesis = Buster, a confirmar) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("BaseDamage", 14),   // AoE FIJO base
        new DynamicVar("PerTen", 10),       // +1 daño por cada 10 de tier (14 + tier/10)
        new DynamicVar("Seal", 1),          // Sello de Habilidad aplicado a todos
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>(),
        HoverTipFactory.FromPower<SkillSealPower>(),
        HoverTipFactory.FromPower<TiamatBeastWindowPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var creature = Owner.Creature;

        // 1) Consume TODA la carga; tier = lo consumido (>= 100). La ventana dura clamp(tier/100,1,3).
        var tier = await NpCharge.ConsumeAllForNpCard(creature, ChargeCost, this);
        var duration = Math.Clamp(tier / 100, 1, 3);

        // 2) Limpia TODOS tus debuffs (preserva recursos y formas vía Cleanse).
        await Cleanse.RemoveDebuffs(creature);

        // 3) AoE FIJO + Sello de Habilidad a todos los enemigos.
        var damage = DynamicVars["BaseDamage"].IntValue + tier / DynamicVars["PerTen"].IntValue;
        foreach (var enemy in creature.CombatState.GetOpponentsOf(creature).ToList())
        {
            if (enemy.IsDead) continue;
            await DamageCmd.Attack(damage).FromCard(this).Targeting(enemy)
                .WithHitFx("vfx/vfx_starry_impact")
                .Execute(choiceContext);
            if (enemy.IsAlive)
            {
                await Sello.Apply(enemy, DynamicVars["Seal"].IntValue, creature, this);
            }
        }

        // 4) Te volvés Bestia II y se manifiesta el mazo Bestia efímero entero.
        await FormSwitch.Enter<TiamatBeastPower>(choiceContext, creature, this);
        await BeastDeck.ManifestAll(creature);

        // 5) Abre la ventana: Counter = duración (la decrementa TiamatBeastWindowPower al inicio de
        //    cada uno de tus turnos; al llegar a 0 revierte a Lily y purga el mazo efímero).
        await PowerCmd.Apply<TiamatBeastWindowPower>(choiceContext, creature, duration, creature, this);

        // 6) Cierre de recursos de la ventana-NP (+1⚡, robar 2) — factorizado en FGOCore.
        await NpWindow.ReturnResources(creature, 1, 2);
    }
}
