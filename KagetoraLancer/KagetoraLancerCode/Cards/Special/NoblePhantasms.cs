using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Special;

public interface IKagetoraNpCard : ICommandTyped { }

public abstract class KagetoraNpCard() : KagetoraCard(
    0, CardType.Attack, CardRarity.Event, TargetType.AnyEnemy), IKagetoraNpCard
{
    public const int ChargeCost = 100;
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    // Canal 5: los NP no tienen precepto, así que el término de la base es constantemente falso y
    // este override manda tal cual (§4.5). El `IsMutable` es la misma guarda que la base: este
    // getter es público y se consulta fuera de combate, donde `Owner` sobre un modelo canónico tira
    // `CanonicalModelException` (AbstractModel.cs:113, `AssertMutable()`) — que ningún `?.` atrapa.
    protected override bool ShouldGlowGoldInternal => IsMutable && IsPlayable;

    // Canal 4 (§4.4): va en `PreceptHoverTips`, NO en `ExtraHoverTips` — sobrescribir `ExtraHoverTips`
    // reemplazaría la lista de `KagetoraCard` y los dos NP perderían el tooltip de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<BishamontenBlessingPower>()];

    protected async Task<int> ConsumeEffectiveTier(PlayerChoiceContext context)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(context, Owner.Creature, ChargeCost, this);
        return Math.Clamp(tier, ChargeCost, 500);
    }

    protected static Task RemoveOffensiveBuffs(
        MegaCrit.Sts2.Core.Entities.Creatures.Creature target) =>
        Cleanse.RemoveOffensiveBuffs(target);
}

/// <summary>
/// §9 — el NP de Kagetora queda [REUSA] **sin cambios de estructura**: 8 impactos,
/// <c>perHit = 3 + Lv</c>, limpieza de mejoras ofensivas DESPUÉS del daño, Débil 1/2/3 por OC y
/// ascenso al final. El OC compra Débil, no daño: banquear en vez de disparar sigue siendo una
/// jugada legítima. Su daño baja de ~91 a ~85 sin tocar una línea, por el cap de la Bendición (E8).
/// </summary>
public sealed class BitenHassouKurumaGakariNoJin() : KagetoraNpCard
{
    private const int Hits = 8;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Hits", Hits)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        var tier = await ConsumeEffectiveTier(context);
        var level = Math.Clamp(NpLevels.Get(Owner), 1, NpLevels.MaxLevel(Owner));
        var perHit = 3m + level;

        for (var hit = 0; hit < Hits && !target.IsDead; hit++)
        {
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
            await CreatureCmdCompatibility.Damage(
                context, target, perHit, ValueProp.Move, Owner.Creature, this, cardPlay);
        }

        // El primer NP limpia después de infligir daño y transforma aunque el objetivo haya muerto.
        await RemoveOffensiveBuffs(target);
        if (!target.IsDead)
        {
            var oc = Math.Clamp(tier / 100, 1, 5);
            var weak = oc >= 5 ? 3m : oc >= 3 ? 2m : 1m;
            await PowerCmd.Apply<WeakPower>(context, target, weak, Owner.Creature, this);
        }

        if (!Owner.Creature.HasPower<KenshinFormPower>())
        {
            await FormSwitch.Enter<KenshinFormPower>(context, Owner.Creature, this);
            await Listeners.ForEachListener<IAscensionListener>(Owner.Creature,
                listener => listener.AfterAscendingToKenshin(context, this));
        }
    }
}

/// <summary>
/// §9 — el NP de Kenshin, re-especificado. Tres cambios, todos por el mismo motivo: **todos los
/// aditivos del kit son POR IMPACTO** (Fuerza, Bendición, Divinidad en el primero). Con 4 impactos
/// Kenshin cobraba la mitad de su propio mazo y hacía 69 donde Kagetora hacía 91.
///
/// 1. **4 → 8 impactos**, igual que el NP de Kagetora.
/// 2. **<c>perHit = 3 + Lv + OC</c>**, con OC = +1 a partir de OC3 y **tope +1** (antes: <c>5 + 2·Lv</c>
///    con +1 por cada nivel de OC sobre 1, hasta +4). El OC compra el salto único de OC3, no una
///    rampa: banquear carga sigue siendo una decisión, no una obligación.
/// 3. **Quita el Bloqueo del objetivo antes del daño**, además de las mejoras ofensivas. Ésta es la
///    ventaja SIEMPRE activa de Kenshin contra jefes; el anti-Man queda como sabor, porque
///    <c>FgoAttributes</c> mapea Elite→Earth y Boss→Heaven y contra los enemigos que justifican un
///    ulti nunca se activa. Es exactamente lo que ya hace el rider de <c>ShiranuiBlade</c>.
///
/// Línea de control (NP3 / +3 Fuerza / Bendición capada a 4 impactos / Divinidad / no-Man):
/// OC1 = 85 (empatado con Kagetora), OC3 = 93, OC5 = 93.
/// </summary>
public sealed class BitenHassouShiranui() : KagetoraNpCard
{
    private const int Hits = 8;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("AntiMan", 3)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        var tier = await ConsumeEffectiveTier(context);
        var level = Math.Clamp(NpLevels.Get(Owner), 1, NpLevels.MaxLevel(Owner));
        var overcharge = Math.Clamp(tier / 100, 1, 5);
        var perHit = 3m + level + (overcharge >= 3 ? 1m : 0m);
        if (FgoAttributes.Is(target, FgoAttribute.Man)) perHit += DynamicVars["AntiMan"].BaseValue;

        await RemoveOffensiveBuffs(target);
        if (target.Block > 0)
            await CreatureCmdCompatibility.LoseBlock(context, target, target.Block, Owner.Creature);

        for (var hit = 0; hit < Hits && !target.IsDead; hit++)
        {
            VfxCmd.PlayOnCreatureCenter(target, "vfx/vfx_dramatic_stab");
            await CreatureCmdCompatibility.Damage(
                context, target, perHit, ValueProp.Move, Owner.Creature, this, cardPlay);
        }
    }
}
