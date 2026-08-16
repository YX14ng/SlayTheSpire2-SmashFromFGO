using MegaCrit.Sts2.Core.CardSelection;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Common;

/// <summary>
/// Canal 5 (§4.5) para los riders «+10★ si fue Crítica»: predicción PURA de crítico, con la MISMA
/// guarda de null/canónico que <c>KagetoraCard.DoctrineEngine</c>. <c>ShouldGlowGold</c> es un
/// getter público que el juego consulta fuera de combate (compendio, pantalla de recompensa), donde
/// <c>Owner</c> es null — y sobre un modelo canónico <c>CardModel.Owner</c> ni siquiera devuelve
/// null: abre con <c>AssertMutable()</c> y tira <c>CanonicalModelException</c>, que el <c>?.</c> NO
/// atrapa. Por eso el <c>IsMutable</c> va PRIMERO.
///
/// <c>Criticals.WillCrit</c> es puro (consulta <c>ICriticalAccessRule</c> —o sea el gate E6 de
/// <c>DoctrinePower</c>— y el banco de estrellas; no gasta nada).
/// </summary>
internal static class CritGlow
{
    public static bool WillCrit(KagetoraCard card) =>
        card.IsMutable && card.Owner?.Creature is { } owner && Criticals.WillCrit(owner, card);
}

public sealed class CelestialThrust() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(7m, ValueProp.Move), new DynamicVar("NpCharge", 10), new DynamicVar("KenshinNpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        // Rider de forma (§5): 9 de las 74 cartas responden al ascenso; ésta paga en Carga NP porque
        // el arquetipo A vive de ciclar Cielo barato hacia el NP, no de daño frontal.
        var npCharge = DynamicVars["NpCharge"].IntValue +
            (Owner.Creature.HasPower<KenshinFormPower>() ? DynamicVars["KenshinNpCharge"].IntValue : 0);
        await NpCharge.Gain(c, Owner.Creature, npCharge, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class FieldReading() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2), new DynamicVar("Discard", 1), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        // ToList + lectura ANTES del descarte: `FromHandForDiscard` devuelve un IEnumerable y
        // `CardCmd.Discard` muta la mano; re-enumerar después podría dar otra cosa.
        var selected = (await CardSelectCmd.FromHandForDiscard(c, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this)).ToList();
        // Conectividad dura (§7.1): la carta tiene que LEER o ESCRIBIR un recurso propio. «Etiquetada»
        // = con precepto 天/胸/足; las neutrales (Encarnación, Doctrina del General, Divinidad) NO
        // cuentan, igual que en la Doctrina.
        var discardedTagged = selected.Any(card => card is IPreceptCard { Precept: not Precept.None });
        await CardCmd.Discard(c, selected);
        if (discardedTagged) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}

public sealed class PrayerToBishamonten() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    // E5: un 0⚡ REPETIBLE tiene que ser una conversión que gasta más de lo que su propio avance
    // devuelve (20★ salen, 10★ vuelven por la Pagoda). Por eso pierde Agotar: la fricción ya la pone
    // el banco de estrellas, no el descarte de la carta.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20), new DynamicVar("NpCharge", 30)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["Stars"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await CritStars.Spend(c, Owner.Creature, DynamicVars["Stars"].IntValue, this)) return;
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(20m);
}

public sealed class TurnTheReins() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    // E5 / 4.6.2: mitad del par espejo a 0⚡ SIN Agotar (la otra es Dar Vuelta a la Formación, que
    // hace el viaje inverso 50 NP → 50★). El gate de 50★ es el precio.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 50), new DynamicVar("NpCharge", 50)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["Stars"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await CritStars.Spend(c, Owner.Creature, DynamicVars["Stars"].IntValue, this)) return;
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(20m);
}

public sealed class BattleOrder() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    // J-02: la mejora YA NO baja el coste a 0⚡ — un tutor a 0⚡ sin gasto viola E5. Paga en Carga NP.
    // El var arranca en 0 y la loc esconde la cláusula sin mejorar (mismo patrón que el `CardsVar(0)`
    // que tenía esta familia y que el fix «Draw 0.» de §11.2).
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 0)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (DoctrineEngine is { } doctrine)
        {
            var candidate = PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(card =>
                card is IPreceptCard tagged && doctrine.WouldAdvanceAfter(Precept.Heaven, tagged.Precept));
            if (candidate != null) await CardPileCmd.Add(candidate, PileType.Hand);
        }
        if (DynamicVars["NpCharge"].IntValue > 0)
            await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class GeneralsCounsel() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class CommandersStaff() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    // ⟳ §7.1: era un Ataque pelado con rider de forma, y «tener rider de forma NO es conectividad».
    // La Carga NP la conecta al motor de verdad.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("NpCharge", 10), new DynamicVar("KenshinBonus", 4)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var damage = DynamicVars.Damage.BaseValue + (Owner.Creature.HasPower<KenshinFormPower>() ? DynamicVars["KenshinBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class ArmourIsInTheChest() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    // ⟳ §7.1: el rider de estrellas es lo que la conecta al motor (el de forma no cuenta).
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(7m, ValueProp.Move), new DynamicVar("KenshinBlock", 3), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        // Se lee ANTES de cualquier efecto: `AfterCardPlayed` corre después de OnPlay, así que acá
        // `WouldAdvanceNow` sigue valiendo lo mismo que doró el borde (§4.5).
        var advances = WouldAdvanceNow;
        var block = DynamicVars.Block.BaseValue +
            (Owner.Creature.HasPower<KenshinFormPower>() ? DynamicVars["KenshinBlock"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, p);
        if (advances) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class DrinkAmongBullets() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(5m, ValueProp.Move), new PowerVar<WeakPower>("Weak", 1m), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var advances = WouldAdvanceNow;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        await PowerCmd.Apply<WeakPower>(c, p.Target, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        if (advances) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class KasugayamaGuard() : KagetoraCard(2, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var advances = WouldAdvanceNow;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        // §17: Bloqueo → Carga NP, una de las dos salidas que sacan al medidor del estancamiento.
        if (advances) await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5m);
}

public sealed class SixPlateCuirass() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new DynamicVar("FirstChestBonus", 3)];

    /// <summary>
    /// «Tu primera Pecho del turno» = el precepto Pecho todavía no avanzó este turno. Sin motor NO
    /// hay bonus: es la misma disciplina del `?? 0` que arregló WallOfBanners (ausencia de power ⇒
    /// ausencia de regalo, nunca al revés).
    /// </summary>
    private bool IsFirstChestOfTurn =>
        DoctrineEngine is { } doctrine && !doctrine.AdvancedThisTurn(Precept.Chest);

    // Canal 5 (§4.5): condición PROPIA además del precepto ⇒ se compone con el término de la base,
    // no lo pisa. (Puede avanzar sin ser la primera Pecho y viceversa: son condiciones distintas.)
    protected override bool ShouldGlowGoldInternal => WouldAdvanceNow || IsFirstChestOfTurn;

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var block = DynamicVars.Block.BaseValue + (IsFirstChestOfTurn ? DynamicVars["FirstChestBonus"].BaseValue : 0m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, p);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class InterposeTheSpear() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    // §12.3-4: queda a 1⚡ (resolución al juez más restrictivo). El 0⚡ repetible de Pecho es
    // Formación Cerrada, que sí gasta estrellas. El robo se cambió por Carga NP (P-2: el cierre
    // paga ⚡ O carta, nunca las dos).
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move), new DynamicVar("NpCharge", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var advances = WouldAdvanceNow;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (advances) await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class SaltForTheRival() : KagetoraCard(1, CardType.Skill, CardRarity.Common, TargetType.AnyPlayer, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(8m, ValueProp.Move), new DynamicVar("RemoveWeak", 1), new DynamicVar("NpCharge", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await CreatureCmd.GainBlock(target, DynamicVars.Block, p);
        // La Carga NP es SIEMPRE de quien juega la carta (mismo contrato co-op que VanguardMandate):
        // el rider cambia números, no estructura (§16.9).
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (target.GetPower<WeakPower>() is not { } weak) return;
        var remove = IsUpgraded ? (int)weak.Amount : DynamicVars["RemoveWeak"].IntValue;
        if (weak.Amount <= remove) await PowerCmd.Remove(weak);
        else await PowerCmd.ModifyAmount(c, weak, -remove, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class ClosedFormation() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Chest)
{
    // Re-efecto TOTAL (§10): era 0⚡ con Agotar y una lectura muerta de `.Block`. Ahora es la
    // conversión estrellas → Bloqueo, y ocupa el hueco que dejó FrontArmour al cortarse (J-08).
    // E5: gasta 20★ y su propio avance sólo devuelve 10★ (Pagoda) ⇒ negativo por construcción.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20), new BlockVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["Stars"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await CritStars.Spend(c, Owner.Creature, DynamicVars["Stars"].IntValue, this)) return;
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    }
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

/// <summary>
/// [NUEVA] Muro de Lanzas — K11/§7.1: Pecho deja de ser el único precepto sin Ataques (0 en 22).
/// §16.6: con E8 los Ataques de Pecho NO arman ni queman la Bendición de Bishamonten; ése es
/// exactamente el punto del parche. Ojo con la reliquia Armadura de Seis Placas («la primera CARTA
/// de Pecho del turno»): ahora puede dispararse con un Ataque. Es coherente y va al changelog.
/// </summary>
public sealed class SpearWall() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Chest), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(5m, ValueProp.Move), new BlockVar(5m, ValueProp.Move), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var advances = WouldAdvanceNow;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        if (advances) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars.Block.UpgradeValueBy(2m); }
}

/// <summary>
/// [NUEVA] Muralla de Echigo — la salida CHICA de Bloqueo → daño (§12.3-8, J-10: ¼ con tope 8 en
/// común; la grande es Guardia Compartida, ⅓ con tope 12, en poco común). Los tres caps se
/// recalibran JUNTOS, nunca de a uno: si éste se toca, se tocan los otros dos (knob §15.4-8).
/// El tope es lo que impide que apilar defensa sea una ruta de daño ilimitada (§14.3).
/// </summary>
public sealed class EchigoRampart() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Chest), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    // La `DamageVar` ES el tope, y por eso el preview no miente: el cap se aplica al valor BASE y la
    // Fuerza suma DESPUÉS, así que `!D!` con hooks aplicados es exactamente el daño máximo real.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move), new DamageVar(8m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        // El Bloqueo se suma ANTES de medir: la muralla pega con lo que acaba de levantar.
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
        var damage = Math.Min(Owner.Creature.Block / 4, (int)DynamicVars.Damage.BaseValue);
        if (damage <= 0) return;
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_attack_slash").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Block.UpgradeValueBy(3m); DynamicVars.Damage.UpgradeValueBy(3m); }
}

public sealed class EightPetalSpear() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    // ⟳ §7.1: el rider de crítico es su conectividad real (el de forma no cuenta).
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("KenshinBonus", 4), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override bool ShouldGlowGoldInternal => WouldAdvanceNow || CritGlow.WillCrit(this);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var damage = DynamicVars.Damage.BaseValue + (Owner.Creature.HasPower<KenshinFormPower>() ? DynamicVars["KenshinBonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        // E6 concentra el único crítico del turno en Pies: devolver estrellas ahí es lo que impide
        // que el banco se vacíe de golpe y el ciclo siguiente arranque de cero.
        if (Criticals.IsCritical(p)) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class HoushoutsukigeCharge() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    // 2⚡ → 1⚡: multi-impacto anti-Buffer en rareza COMÚN (§2). A 2⚡ cancelaba el ciclo del turno.
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override bool ShouldGlowGoldInternal => WouldAdvanceNow || CritGlow.WillCrit(this);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        if (Criticals.IsCritical(p)) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class StepOfVictory() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Feet)
{
    // E5, tercera rama: un 0⚡ de VALOR NETO (no convierte nada, sólo genera) lleva Agotar. Sin
    // Agotar sería la única fuente repetible de estrellas gratis del pool.
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class TurnTheFormation() : KagetoraCard(0, CardType.Skill, CardRarity.Common, TargetType.Self, Precept.Feet)
{
    // 4.6.2: la otra mitad del par espejo a 0⚡ sin Agotar (ida: Volver las Riendas, 50★ → 50 NP).
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50), new DynamicVar("Stars", 50)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];
    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        if (!await NpCharge.Spend(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this)) return;
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(20m);
}

public sealed class NaginataSweep() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    // ⟳ §7.1: era el único AoE común sin conexión con ningún recurso propio.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("Stars", 10)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).TargetingAllOpponents(Owner.Creature.CombatState!).WithHitFx("vfx/vfx_attack_slash").Execute(c);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(3m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

public sealed class AlternatingAttack() : KagetoraCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    // §7.1 pide «4×2, +10★» y el +10★ YA LO DA EL TIPO: `CommandBonusPower.AfterCardPlayed` concede
    // `QuickStars = 10` a TODA Quick normal (FGOCore/CardTypes/CommandBonusPower.cs:81-83). Sumar
    // aquí un CritStars.Gain explícito daría 20★ — el doble del pool con el que se calibró el banco
    // de 100. Por eso esta carta NO cambia: el número del doc ya está implementado por el motor.
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 2)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 2 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2m);
}

/// <summary>
/// [NUEVA] Carga Estrellada — el sumidero estrellas → daño (§17), y el tercer 0⚡ de la «mano de
/// conversión» auditada en §3.4.
///
/// ⚠️ MEDIDO, no asumido: por ser **Quick** (tipo que §7.1 fija), `CommandBonusPower` le devuelve
/// +10★ automáticos después de resolver (FGOCore/CardTypes/CommandBonusPower.cs:81-83). El gasto
/// EFECTIVO es 10★, no 20★, y la mano de tres 0⚡ de §3.4 cierra en **0★ neto**, no en −10★. E5
/// sigue en pie (gasta más de lo que su propio avance devuelve: −20 contra +10 de Pagoda), pero si
/// el playtest dice que la mano de conversión domina, el knob es §15.4-4 (subir el gate a 30★).
/// </summary>
public sealed class StarlitCharge() : KagetoraCard(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20), new DamageVar(8m, ValueProp.Move)];
    protected override IEnumerable<IHoverTip> PreceptHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];
    protected override bool IsPlayable => CritStars.CanPay(Owner.Creature, DynamicVars["Stars"].IntValue);
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        // Es el ÚNICO conversor de estrellas que además es Ataque, así que compite con el crítico por
        // el mismo banco: `CriticalResolverPower.BeforeCardPlayed` corre ANTES de este OnPlay y puede
        // haberse llevado 50★ (Criticals.cs:154). Con 60★ en banco el cobro doble dejaría la carta en
        // nada. Si el crítico ya pagó, E5 está cumplida con creces y la carta pega igual.
        var paid = await CritStars.Spend(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        if (!paid && !Criticals.IsCritical(p)) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_starry_impact").Execute(c);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
