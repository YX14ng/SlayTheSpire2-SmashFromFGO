using System.Linq;
using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Special;

/// <summary>
/// SENTENCIA DE LA REINA: Desatado (女王的判决·全解放) — la carta-NP auto-manifestada GRATIS al
/// cruzar 100 NP (consistencia con los otros Servants, 2026-06-26; antes era la «ventana» inline
/// de MainFile). 0⚡, Retain + Exhaust, consume TODA la Carga (mín. !ChargeCost!). La cosecha masiva
/// de la Reina: cada enemigo MALDITO recibe daño = su Maldición SIN consumirla (el campo sigue
/// sembrado para tus Ataques de Sentencia). Daño Unblockable|Unpowered (no escala con Fuerza ni lo
/// frena el Bloqueo), idéntico al tick de Maldición. SOBRECARGA: +!PerTen! a cada detonación por
/// cada 10 de Carga sobre el mínimo. Escala +15%/nivel (NpLevels).
/// </summary>
public sealed class QueensSentenceUnleashed() : MorganCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Roadless Camelot = Buster) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(choiceContext, Owner.Creature, ChargeCost, this);
        var bonus = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        // Detonación AoE: cada enemigo maldito recibe daño = su Maldición (+ Sobrecarga), SIN
        // consumirla. Unblockable|Unpowered (no escala con Fuerza ni lo frena el Bloqueo), como
        // el tick de Maldición. Daño directo con ValueProp explícita (patrón del antiguo handler).
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature).Where(e => !e.IsDead).ToList())
        {
            var curse = Curses.Of(enemy);
            if (curse <= 0) continue;
            var damage = NpLevels.Scale(Owner, curse + bonus);
            VfxCmd.PlayOnCreatureCenter(enemy, "vfx/vfx_starry_impact");
            await CreatureCmdCompatibility.Damage(choiceContext, enemy, damage,
                ValueProp.Unblockable | ValueProp.Unpowered, Owner.Creature, this, cardPlay);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["PerTen"].UpgradeValueBy(1m);
    }
}
