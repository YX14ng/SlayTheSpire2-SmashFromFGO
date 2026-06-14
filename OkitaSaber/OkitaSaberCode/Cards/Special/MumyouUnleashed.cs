using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Special;

/// <summary>
/// «Mumyou Sandanzuki: Desatado» (无明三段突·解放) — la carta-NP auto-manifestada GRATIS al cruzar
/// 100 NP (DESIGN-OKITA §5.5). 0⚡, Retain + Exhaust, consume TODA la Carga (mín. 100). 14 daño ×3 a
/// UN enemigo IGNORANDO Bloqueo (Unblockable); tras el daño, 2 Vulnerable. SOBRECARGA: +1/golpe por
/// cada 10 sobre 100 (a banco 300: 34×3 = 102 perforantes). Un *Crítico Listo en cola dobla el 1er
/// golpe (la cola de FGOCore lo aplica solo al 1er golpe de la carta). Escala +15%/nivel (NpLevels).
///
/// MEJORADA (up): si tenés Crítico Listo lo consume y LOS TRES golpes critican; Vulnerable sube a 3.
/// (Implementado como overcharge sobre el daño base + Vulnerable; el all-crit del 1er golpe sale del
/// ×2 nativo de CritReadyPower; el up sube la base y el Vulnerable — el all-crit pleno de los 3 golpes
/// queda como nota de balance §pista 3, fuera del alcance verificado de este hook.)
/// </summary>
public sealed class MumyouUnleashed() : OkitaCard(0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy)
{
    public const int ChargeCost = 100;
    private const int Hits = 3;
    private const int OverchargePerTen = 1; // +1 daño por golpe por cada 10 sobre 100

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move | ValueProp.Unblockable),
        new PowerVar<VulnerablePower>("Vulnerable", 2m),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<CritReadyPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var perHit = (tier - ChargeCost) / 10 * OverchargePerTen;
        var perHitDamage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + perHit);

        // 3 golpes perforantes (IGNORA Bloqueo = Unblockable). El ×2 de un Crítico Listo en cola
        // aplica al daño Unblockable (CritReadyPower no distingue — RhongomyniadReplica §). Daño
        // directo con ValueProp explícita (el builder fluido no expone Unblockable; patrón ConceptualRound).
        for (var i = 0; i < Hits; i++)
        {
            if (cardPlay.Target.IsDead) break;
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_dramatic_stab");
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, perHitDamage,
                ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this);
        }

        // Tras el daño: Vulnerable (potencia el resto del turno — fidelidad al OC real).
        if (!cardPlay.Target.IsDead)
        {
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);          // 14 -> 18
        DynamicVars["Vulnerable"].UpgradeValueBy(1m);    // 2 -> 3
    }
}
