using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Uncommon;

/// <summary>
/// §7.2 / §10 [REUSA re-efecto TOTAL] — era una copia estrictamente dominada de
/// <see cref="VanguardMandate"/> (misma rareza, mismo coste, mismo precepto, mismo robo, y encima
/// con el NP condicionado y sin poder targetear). Ahora ocupa la única arista que le faltaba al
/// grafo de conversión (§17): <b>Carga NP → cartas</b>.
///
/// E5: es 0⚡ y repetible, así que GASTA más de lo que su propio avance devuelve (30 de Carga NP
/// contra los 10 que paga el avance de Cielo) ⇒ no necesita Agotar.
/// </summary>
public sealed class WheelStrategy() : KagetoraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("NpCharge", 30)];

    // `NpCharge.Current` y NO `NpCharge.CanPay`: CanPay devuelve true si hay un INpCostWaiver activo,
    // pero el waiver sólo cubre CARTAS NP — acá `Spend` fallaría igual y la carta se jugaría en vano
    // (jugable en la UI, no-op en la resolución). Precedente: MorganBerserker/Cards/Common/MistVeil.cs:29.
    protected override bool IsPlayable => NpCharge.Current(Owner.Creature) >= DynamicVars["NpCharge"].IntValue;

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await NpCharge.Spend(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this)) return;
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class FourHeavenlyStrikes() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class PrepareTheCavalry() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        var chosen = (await CardSelectCmd.FromHand(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1), card => card != this, this)).FirstOrDefault();
        chosen?.GiveSingleTurnRetain();
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class MagicalCharge() : KagetoraCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 30)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(20m);
}

public sealed class FormationRelay() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (doctrine == null) return;
        // FromChooseACardScreen sirve para sets chicos GENERADOS: tira ArgumentException con más de
        // 3 cartas y el descarte no tiene tope. Para elegir de una pila va FromCombatPile, que
        // recibe el filtro y arma la grilla con TODAS las candidatas, sin recortarlas a 3.
        var pile = PileType.Discard.GetPile(Owner);
        bool CouldAdvance(CardModel card) =>
            card is IPreceptCard tagged && doctrine.WouldAdvanceAfter(Precept.Heaven, tagged.Precept);
        var chosen = (await CardSelectCmd.FromCombatPile(
            c, pile, Owner, new CardSelectorPrefs(SelectionScreenPrompt, 1), CouldAdvance)).FirstOrDefault();
        if (chosen != null) await CardPileCmd.Add(chosen, PileType.Hand);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class CommandersGaze() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>("Weak", 2m), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}

public sealed class VanguardMandate() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyPlayer, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target?.Player ?? Owner;
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, target);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class HeavensFocus() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("CritReady", 1), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await Criticals.GrantReady(c, Owner.Creature, DynamicVars["CritReady"].IntValue, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class ArmourInTheChestA() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<IntangiblePower>(c, Owner.Creature, 1m, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class BulletCurtain() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(9m, ValueProp.Move), new PowerVar<BulletCurtainPower>("Stars", 20m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<BulletCurtainPower>(c, Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class RulersDefense() : KagetoraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(14m, ValueProp.Move), new PowerVar<ArtifactPower>("Artifact", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}

public sealed class SereneCounterattack() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(7m, ValueProp.Move), new PowerVar<SereneCounterPower>("Counter", 6m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<SereneCounterPower>(c, Owner.Creature, DynamicVars["Counter"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(2m); DynamicVars["Counter"].UpgradeValueBy(3m); }
}

public sealed class FearlessChest() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FearlessChestPower>("Block", 2m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<FearlessChestPower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(1m);
}

public sealed class TreasureInTheHeartB() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<ArtifactPower>("Artifact", 2m), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}

/// <summary>
/// §7.2 / §10 [REUSA re-efecto TOTAL] — en solitario era idéntica a <c>ArmourIsInTheChest</c> (común).
/// Ahora es la <b>salida grande de Bloqueo→daño</b> (⅓, tope 12); la chica vive en la común
/// `Muralla de Echigo` (¼, tope 8). Los dos topes se recalibran SIEMPRE juntos (§12.3-8, J-10).
///
/// El rider co-op se conserva en estructura y sólo cambia números (§15.1-8).
/// </summary>
public sealed class SharedGuard() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Chest)
{
    /// <summary>Divisor de la conversión Bloqueo→daño. ⅓, contra el ¼ de la común.</summary>
    private const int BlockDivisor = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, ValueProp.Move), new DynamicVar("AllyBlock", 4), new DynamicVar("MaxDamage", 12)];

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
        {
            var block = creature == Owner.Creature ? DynamicVars.Block.BaseValue : DynamicVars["AllyBlock"].BaseValue;
            await CreatureCmd.GainBlock(creature, block, ValueProp.Move, p);
        }

        // El daño se calcula DESPUÉS de ganar el Bloqueo (lo dice el texto: «7 Bloqueo, después
        // daño = ⅓ de tu Bloqueo»), así que la carta nunca pega 0 aunque entres sin Bloqueo.
        //
        // Va `Unpowered` y SIN `Move` a propósito: `IsPoweredAttack()` es falso, así que Fuerza,
        // Divinidad, la Bendición y el crítico NO la multiplican. Esto es lo que hace que §14.3
        // pueda firmar «apilar defensa nunca es una ruta de daño ilimitada»: el único tope de la
        // conversión es `MaxDamage`, y no hay multiplicadores atrás. Sigue respetando el Bloqueo
        // enemigo (no lleva `Unblockable`).
        var damage = Math.Min(Owner.Creature.Block / BlockDivisor, DynamicVars["MaxDamage"].IntValue);
        if (damage > 0)
            await CreatureCmdCompatibility.Damage(
                c, p.Target, damage, ValueProp.Unpowered, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["MaxDamage"].UpgradeValueBy(4m); }
}

/// <summary>§7.2 [REUSA re-efecto: <b>2⚡→1⚡</b>] — 9 Bloqueo (12) y +20★ (30) si otro precepto ya avanzó.</summary>
public sealed class WallOfBanners() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(9m, ValueProp.Move), new DynamicVar("Stars", 20)];

    /// <summary>
    /// Canal 5 (§4.5, regla 4.6.5): condicional PROPIA ⇒ glow propio, compuesto con el del precepto
    /// que ya trae la base — nunca lo pisa. `DoctrineEngine` es el accesor null-safe: este getter se
    /// consulta fuera de combate (compendio, recompensa) sobre modelos canónicos.
    /// </summary>
    protected override bool ShouldGlowGoldInternal =>
        WouldAdvanceNow || (DoctrineEngine is { } doctrine && OtherPreceptAdvanced(doctrine.AdvancedMaskThisTurn));

    // `?? 0` obligatorio en el call-site: sin él, `null & N` es null y `null != 0` es TRUE — sin
    // DoctrinePower la carta regalaba las estrellas (mismo defecto que KagetoraUsages.WasUsed,
    // 2026-08-16). Acá el mask ya llega saneado desde el accesor.
    private static bool OtherPreceptAdvanced(int advancedMask) => (advancedMask & ~(int)Precept.Chest) != 0;

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (OtherPreceptAdvanced(Owner.Creature.GetPower<DoctrinePower>()?.AdvancedMaskThisTurn ?? 0))
            await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

/// <summary>§7.2 [REUSA re-efecto: <b>2⚡→1⚡</b>] — a 2⚡ cancelaba el ciclo del turno que la jugabas.</summary>
public sealed class JustPath() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<JustPathPower>("Block", 6m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<JustPathPower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}

/// <summary>
/// §7.2 / §10 / §12.3-5 [REUSA re-efecto] — <b>delta silencioso al changelog</b> (§16.8): deja de dar
/// +2/+3 de Fuerza y da +1 fijo; la mejora se mudó a las estrellas.
///
/// Por qué: la Fuerza multiplica ~20 impactos por turno (§14.1-4). Con +2/+3 acá, NINGUNA
/// combinación de las contingencias de los tres jueces bajaba el pico de 245 contra un techo de
/// 180-220. Este parche y el tope de Manifestación (+3) son los dos que hacen cerrar §14.
/// </summary>
public sealed class MeritIsInTheFeetA() : KagetoraCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("Strength", 1m), new DynamicVar("AllyStrength", 1), new DynamicVar("Stars", 30)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        foreach (var creature in Owner.Creature.CombatState!.PlayerCreatures)
            await PowerCmd.Apply<StrengthPower>(
                c, creature,
                creature == Owner.Creature ? DynamicVars["Strength"].BaseValue : DynamicVars["AllyStrength"].BaseValue,
                Owner.Creature, this);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    // La mejora NO toca la Fuerza: ése es exactamente el escalado que se cortó.
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(20m);
}

public sealed class HoushoutsukigeGallop() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("KenshinHits", 4), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        // Rider de forma (§5): una de las 9 cartas que hacen visible la ascensión. Un impacto más, no
        // más daño por impacto — Kenshin cobra en impactos porque todos los aditivos del kit
        // (Fuerza, Bendición, Divinidad) son POR IMPACTO (§9).
        var hits = Owner.Creature.HasPower<KenshinFormPower>()
            ? DynamicVars["KenshinHits"].IntValue
            : DynamicVars["Hits"].IntValue;
        for (var i = 0; i < hits && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

/// <summary>
/// §7.2 / §11.3 [REUSA re-efecto: <b>2⚡→1⚡</b> + <b>fix</b>] — <b>delta silencioso al changelog</b>
/// (§16.8): 10×2 → 7×2, es decir <b>−30 % de daño base</b>, no sólo un cambio de coste.
///
/// El fix es el bug P2 de §11.3: leía <c>AdvancedMaskThisTurn</c> ANTES de su propio avance. Como
/// <c>AfterCardPlayed</c> corre después del texto de la carta, su propio Pies nunca estaba en el
/// mask ⇒ techo real +10★ contra los +30★ que prometía el texto. Ahora usa
/// <see cref="DoctrinePower.AdvancedMaskIncludingThisPlay"/>, que cuenta el propio precepto.
/// </summary>
public sealed class EightWeaponsOneWarrior() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(7m, ValueProp.Move), new DynamicVar("Hits", 2), new DynamicVar("StarsPer", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < DynamicVars["Hits"].IntValue && !p.Target.IsDead; i++)
            await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        if (Owner.Creature.GetPower<DoctrinePower>() is not { } doctrine) return;
        var count = DoctrinePower.PreceptCount(doctrine.AdvancedMaskIncludingThisPlay(Precept.Feet));
        if (count > 0) await CritStars.Gain(c, Owner.Creature, count * DynamicVars["StarsPer"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

public sealed class SpinningNaginata() : KagetoraCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(8m, ValueProp.Move), new DynamicVar("Stars", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class RelentlessPursuit() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(11m, ValueProp.Move), new DynamicVar("Bonus", 5)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var hasBuff = p.Target.GetPowerInstances<PowerModel>().Any(power => power.Type == PowerType.Buff);
        var damage = DynamicVars.Damage.BaseValue + (hasBuff ? 0m : DynamicVars["Bonus"].BaseValue);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Bonus"].UpgradeValueBy(2m); }
}

public sealed class AlternatingAssault() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(3m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 20)];

    // Canal 4: el rider lee el crítico, así que la carta explica el crítico. Va en PreceptHoverTips
    // y NO en ExtraHoverTips: sobrescribir ExtraHoverTips reemplazaría la lista de la base y esta
    // carta perdería el tooltip de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    /// <summary>
    /// Canal 5 (§4.5, regla 4.6.5): «si fue Crítica» es una condicional propia ⇒ glow propio,
    /// compuesto con el del precepto. <c>Criticals.WillCrit</c> es la predicción PURA que FGOCore
    /// expone justo para hover/glow, y ya respeta el cap de un crítico por turno de E6
    /// (<c>CanSpend</c> → <c>ICriticalAccessRule</c>), así que el borde deja de dorarse cuando el
    /// crítico del turno ya se gastó. Guarda de null completa (J-05): fuera de combate no hay dueño.
    /// </summary>
    protected override bool ShouldGlowGoldInternal =>
        WouldAdvanceNow ||
        (IsMutable && Owner?.Creature is { } creature && Criticals.WillCrit(creature, this));

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < DynamicVars["Hits"].IntValue && !p.Target.IsDead; i++)
            await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        if (Criticals.IsCritical(p)) await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(1m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

/// <summary>
/// §7.2 [REUSA re-efecto: <b>2⚡→1⚡</b>] — 18 → 12 daño (mejora 16). Es una de las dos jugadas que
/// el refund del ciclo tiene que poder pagar en el mismo turno (§3.3, escenario «cerrar y rematar»);
/// a 2⚡ eso era imposible.
/// </summary>
public sealed class RetreatIsHell() : KagetoraCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(12m, ValueProp.Move), new DynamicVar("Stars", 30)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var alive = p.Target.IsAlive;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (alive && p.Target.IsDead) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(4m); DynamicVars["Stars"].UpgradeValueBy(20m); }
}

public sealed class RidingC() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<RidingPower>("Stars", 10m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<RidingPower>(c, Owner.Creature, 10m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class ArmyFootsteps() : KagetoraCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        // BUGFIX 2026-08-16: tutoreaba un Pies A SECAS. Como esta carta ES Pies y ya avanzó al
        // resolverse, en Kagetora (orden fijo) la carta traída NUNCA podía avanzar → tutor inútil.
        // Ahora usa WouldAdvanceAfter como BattleOrder (CommonCards.cs:69-72): trae la primera que
        // SÍ avanzaría después de esta. Sirve en ambas formas.
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (doctrine == null) return;
        var card = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(x =>
            x is IPreceptCard tagged && doctrine.WouldAdvanceAfter(Precept.Feet, tagged.Precept));
        if (card != null) await CardPileCmd.Add(card, PileType.Hand);
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class GeneralsDoctrine() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<GeneralsDoctrinePower>("Block", 3m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<GeneralsDoctrinePower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}

/// <summary>
/// §7.2 / §12.2 (P-5) [REUSA re-efecto: <b>2⚡→1⚡</b> + <b>cap</b>] — +3 al primer impacto de UN
/// Ataque por turno (+5 como Kenshin); la mejora sube el número (+4/+6) en vez de bajar el coste,
/// que a 1⚡ ya no tendría a dónde ir.
///
/// ⚠️ El <b>cap de 1 Ataque por turno</b> NO vive acá: <c>DivinityPower</c> se re-arma en cada
/// <c>CardPlay</c> (§14.1-2), así que el cap va en el power, con el bit
/// <c>KagetoraUsage.Divinity = 8192</c> de §16.3. Ese archivo (<c>Powers/UncommonPowers.cs</c>) no
/// pertenece a este lote.
/// </summary>
public sealed class DivinityCToA() : KagetoraCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<DivinityPower>("Damage", 3m)];
    // La var y no un 3m hardcodeado: ahora la mejora la mueve, y el power lee su propio Amount.
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<DivinityPower>(c, Owner.Creature, DynamicVars["Damage"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Damage"].UpgradeValueBy(1m);
}
