using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

/// <summary>
/// BALMUNG: Desatado (幻想大剑·天魔失坠) — la carta-NP auto-manifestada GRATIS al cruzar 100 NP
/// (consistencia con los otros Servants, 2026-06-26). 0⚡, Retain + Exhaust, consume TODA la Carga
/// (mín. !ChargeCost!). AoE Buster de Siegfried: golpea a TODOS los enemigos con daño que escala
/// con la SOBRECARGA, fiel a la armadura (DESIGN-SIEGFRIED §4): +!PerTen! por cada 10 de Carga
/// sobre el mínimo, o +!PerTenScaled! EN SU LUGAR mientras tu Sangre de Dragón ≥ 3 — "la sangre del
/// dragón afila el filo" (él ES el dragón). Escala +15%/nivel (NpLevels).
///
/// Gemela auto-manifestada de la rara <see cref="Rare.Balmung"/> (la versión "comprada" de 2⚡ con
/// refund EX): misma identidad y mismo escalado de SdD, pero esta aparece sola al pico y NO se
/// mejora (sin rider de refund). Es la ulti del sistema (ICommandTyped + IsNoblePhantasm).
/// </summary>
public sealed class BalmungUnleashed() : SiegfriedCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), ISiegfriedNpCard, ICommandTyped
{
    public const int ChargeCost = 100;

    private const int OverchargePerTen = 1;       // +1 daño por cada 10 sobre el mínimo
    private const int OverchargePerTenScaled = 2; // +2 en su lugar si SdD >= 3
    private const int ScaledThreshold = 3;        // SdD a partir del cual el filo se afila

    // TAREA D: tipo de NP del juego original (Balmung = Buster) → bonus reforzado de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", OverchargePerTen),
        new DynamicVar("PerTenScaled", OverchargePerTenScaled)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<DragonScalesPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);
    // Resplandor dorado cuando es jugable Y la armadura afila el filo (SdD >= 3).
    protected override bool ShouldGlowGoldInternal => IsPlayable && GlowAtScales(ScaledThreshold);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var scaled = Scales >= ScaledThreshold;
        var perTen = scaled ? OverchargePerTenScaled : OverchargePerTen;
        var overcharge = (tier - ChargeCost) / 10 * perTen;

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge);
        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }
}
