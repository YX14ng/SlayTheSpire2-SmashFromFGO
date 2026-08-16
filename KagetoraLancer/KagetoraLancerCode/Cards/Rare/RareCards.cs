using KagetoraLancer.KagetoraLancerCode.Cards.Special;
using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Rare;

/// <summary>
/// §7.3 — 2⚡→1⚡. La mejora YA NO baja el coste: a 1⚡ lo dejaría en 0⚡, y un Poder que a 0⚡ paga
/// todos los turnos es exactamente la conversión gratuita que E5 prohíbe. La mejora se muda al
/// caudal de estrellas (10★ → 20★ al inicio de turno).
///
/// Por eso el <c>Amount</c> del power pasa a ser LAS ESTRELLAS por turno (era la Carga NP): es el
/// único de los dos números que la mejora mueve. El +10 de Carga NP del primer avance de Cielo es
/// fijo y vive como constante en <see cref="WhiteFlamePower.HeavenAdvanceNp"/>.
/// </summary>
public sealed class WhiteFlameA() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<WhiteFlamePower>("Stars", 10m), new DynamicVar("NpCharge", WhiteFlamePower.HeavenAdvanceNp)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) =>
        PowerCmd.Apply<WhiteFlamePower>(c, Owner.Creature, DynamicVars["Stars"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class JeweledPagodaC() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<StrengthPower>("Strength", 1m), new DynamicVar("NpCharge", 20), new DynamicVar("Overcharge", 2)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await PowerCmd.Apply<StrengthPower>(c, target, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        // NO "arreglar" el 1m a 2m: `OverchargePreparationPower.ExtraTier = 200` y un nivel de OC son
        // 100 de tier, así que UNA carga ya son los +2 niveles que declara `!Overcharge!`. Su
        // `MaxStacks = 1` cierra la cuenta. §7.3 la deja [REUSA] sin cambios.
        if (target.HasPower<CommandBonusPower>() || target.HasPower<NpChargePower>())
            await PowerCmd.Apply<OverchargePreparationPower>(c, target, 1m, Owner.Creature, this);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars["Strength"].UpgradeValueBy(1m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

// §7.3 — 2⚡→1⚡, mejora a 0⚡. E5 no aplica: un Poder se juega una vez, no es una conversión repetible.
public sealed class EightFormationsOfBishamonten() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EightFormationsPower>("EightFormations", 1m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<EightFormationsPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class WisdomOfEightyFourThousandTeachings() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(4), new DynamicVar("NpCharge", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
        await NpCharge.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

public sealed class VowOfBishamonten() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Heaven)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50), new PowerVar<ArtifactPower>("Artifact", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await NpCharge.Gain(c, Owner.Creature, 50, this);
        await PowerCmd.Apply<ArtifactPower>(c, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
    }
    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}

public sealed class WhiteFlameColdAndBurning() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move), new DynamicVar("Hits", 3), new DynamicVar("NpCharge", 20), new PowerVar<VulnerablePower>("Vulnerable", 2m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 3 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await NpCharge.Gain(c, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (!p.Target.IsDead) await PowerCmd.Apply<VulnerablePower>(c, p.Target, 2m, Owner.Creature, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(2m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class TwoRulerEvasions() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>("Buffer", 2m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<BufferPower>(c, Owner.Creature, DynamicVars["Buffer"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["Buffer"].UpgradeValueBy(1m);
}

// §7.3 — 2⚡→1⚡. La mejora sigue siendo la Carga NP (10→20), no el coste.
public sealed class TreasureIsInTheHeart() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<TreasureInHeartPower>("NpCharge", 10m)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<TreasureInHeartPower>(c, Owner.Creature, DynamicVars["NpCharge"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

public sealed class SendSaltToTheEnemy() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.AnyPlayer, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HealVar(6m), new BlockVar(12m, ValueProp.Move)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        var target = p.Target ?? Owner.Creature;
        await CreatureCmd.Heal(target, DynamicVars.Heal.BaseValue);
        await CreatureCmd.GainBlock(target, DynamicVars.Block.BaseValue, ValueProp.Move, p);
    }
    protected override void OnUpgrade() { DynamicVars.Heal.UpgradeValueBy(3m); DynamicVars.Block.UpgradeValueBy(4m); }
}

public sealed class WallsOfKasugayama() : KagetoraCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(20m, ValueProp.Move)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, p);
    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(6m);
}

// §7.3 — 2⚡→1⚡. La mejora sigue siendo 8→12 Bloqueo y 10→20 Carga NP.
public sealed class FieldJudge() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<FieldJudgePower>("Block", 8m), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<FieldJudgePower>(c, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    protected override void OnUpgrade() { DynamicVars["Block"].UpgradeValueBy(4m); DynamicVars["NpCharge"].UpgradeValueBy(10m); }
}

public sealed class SipAtTheCenterOfTheArmy() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self, Precept.Chest)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<IntangiblePower>("Intangible", 1m), new DynamicVar("Stars", 20), new CardsVar(1)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        await PowerCmd.Apply<IntangiblePower>(c, Owner.Creature, 1m, Owner.Creature, this);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await CardPileCmd.Draw(c, DynamicVars.Cards.IntValue, Owner);
    }
    protected override void OnUpgrade() { DynamicVars["Stars"].UpgradeValueBy(10m); DynamicVars.Cards.UpgradeValueBy(1m); }
}

/// <summary>
/// §7.3 + P-8 + §15.1-5 — 2×8 → **2×6**, y la mejora deja de tocar los impactos: pasa a +10★.
/// Es la carta que cobra el único crítico del turno (E6), y el crítico es ×1,5 multiplicativo
/// sobre Fuerza + Divinidad + Bendición POR IMPACTO: cada impacto de más vale mucho más de lo que
/// dice su daño base. Bajar impactos y congelar la mejora es lo que mantiene el pico dentro del
/// techo (§14.2 la cuenta a 6 impactos).
/// </summary>
public sealed class BitenWheelFormation() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    private const int Hits = 6;
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(2m, ValueProp.Move), new DynamicVar("Hits", Hits), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < Hits && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(10m);
}

public sealed class ShiranuiBlade() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var kenshin = Owner.Creature.HasPower<KenshinFormPower>();
        if (kenshin && p.Target.Block > 0)
            await CreatureCmdCompatibility.LoseBlock(c, p.Target, p.Target.Block, Owner.Creature);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
        if (!kenshin) await CritStars.Gain(c, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(6m); DynamicVars["Stars"].UpgradeValueBy(10m); }
}

// §7.3/§10 — 3⚡→2⚡: a 3⚡ era el turno entero ⇒ cancelaba el ciclo, y salía peor por energía que
// una común. Es la única carta de área de las raras. Tras este pase el pool no tiene ningún 3⚡.
public sealed class FullHoushoutsukigeGallop() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(5m, ValueProp.Move), new DynamicVar("Hits", 3)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        for (var hit = 0; hit < 3; hit++)
        foreach (var target in Owner.Creature.CombatState!.HittableEnemies.ToList())
            if (!target.IsDead) await CreatureCmdCompatibility.Damage(c, target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

public sealed class Kawanakajima() : KagetoraCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(20m, ValueProp.Move), new DynamicVar("Bonus", 8)];

    /// <summary>
    /// Canal 5 (regla 4.6.5): esta carta tiene una condicional PROPIA además del precepto, así que
    /// el glow se COMPONE con el de la base en vez de pisarlo. Guarda de null completa (§4.5): el
    /// getter es público y se consulta en el compendio y en la pantalla de recompensa, donde no hay
    /// combate — y sobre un modelo canónico `Owner` tira `CanonicalModelException`, que el `?.` no
    /// atrapa; de ahí el `IsMutable` al frente.
    /// </summary>
    private bool IsMajorEncounter =>
        IsMutable && Owner?.Creature?.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss;

    protected override bool ShouldGlowGoldInternal => base.ShouldGlowGoldInternal || IsMajorEncounter;

    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        var major = IsMajorEncounter;
        var damage = DynamicVars.Damage.BaseValue + (major ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, p).Targeting(p.Target).WithHitFx("vfx/vfx_dramatic_stab").Execute(c);
    }
    protected override void OnUpgrade() { DynamicVars.Damage.UpgradeValueBy(6m); DynamicVars["Bonus"].UpgradeValueBy(2m); }
}

public sealed class EightWeaponsUnleashed() : KagetoraCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move), new DynamicVar("Hits", 4), new DynamicVar("Stars", 20)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        ArgumentNullException.ThrowIfNull(p.Target);
        for (var i = 0; i < 4 && !p.Target.IsDead; i++) await CreatureCmdCompatibility.Damage(c, p.Target, DynamicVars.Damage.BaseValue, ValueProp.Move, Owner.Creature, this, p);
        await CritStars.Gain(c, Owner.Creature, 20, this);
    }
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}

// §7.3 — 2⚡→1⚡. La mejora sigue siendo el nivel del power (Amount 2 = además +10 Carga NP).
public sealed class VictoryIsInTheFeet() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self, Precept.Feet)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VictoryIsInTheFeetPower>("Victory", 1m), new DynamicVar("Stars", 20), new DynamicVar("NpCharge", 10)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<VictoryIsInTheFeetPower>(c, Owner.Creature, IsUpgraded ? 2m : 1m, Owner.Creature, this);
    protected override void OnUpgrade() { }
}

/// <summary>
/// §7.3 — 2⚡→1⚡, mejora a 0⚡ (Agotar: no es repetible, E5 no aplica).
///
/// §11.3 «fix de contrato», mitad hecha: la carta MUTA su propio <c>Precept</c> porque el motor lee
/// <c>tagged.Precept</c> en <c>AfterCardPlayed</c> y el camino de <c>IDoctrineFailureOverride</c>
/// exige <c>precept != None</c> — con <c>Precept.None</c> el motor sale antes de consultar overrides
/// (`Doctrine.cs`, guard de `AfterCardPlayed`). Mientras el binding no se mude a
/// <c>ForcedDoctrineAdvancePower</c>, acá se corta al menos la PERSISTENCIA: el precepto se limpia
/// SIEMPRE al entrar, así una elección cancelada o un combate anterior no dejan la carta convertida
/// en «de Pecho» para el resto de la run (glow y avance mentían). Ver el reporte del lote.
/// </summary>
public sealed class FortuneArmourAndMeritA() : KagetoraCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BufferPower>("Buffer", 1m)];
    protected override async Task OnPlay(PlayerChoiceContext c, CardPlay p)
    {
        Precept = Precept.None;
        var state = Owner.Creature.CombatState;
        var doctrine = Owner.Creature.GetPower<DoctrinePower>();
        if (state != null && doctrine != null)
        {
            var options = new List<CardModel>();
            if ((doctrine.ProgressMask & 1) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseHeaven>(), Owner));
            if ((doctrine.ProgressMask & 2) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseChest>(), Owner));
            if ((doctrine.ProgressMask & 4) == 0) options.Add(state.CreateCard(ModelDb.Card<ChooseFeet>(), Owner));
            var selected = await CardSelectCmd.FromChooseACardScreen(c, options, Owner, false);
            if (selected is IPreceptChoice choice)
            {
                Precept = choice.ChosenPrecept;
                await PowerCmd.Apply<ForcedDoctrineAdvancePower>(c, Owner.Creature, 1m, Owner.Creature, this, silent: true);
                Owner.Creature.GetPower<ForcedDoctrineAdvancePower>()?.Arm(this);
            }
        }
        await PowerCmd.Apply<BufferPower>(c, Owner.Creature, 1m, Owner.Creature, this);
    }
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}

/// <summary>
/// §7.3 — 2⚡→1⚡, mejora a 0⚡. El «tope acumulado +3» de §12.3-5 NO necesita contador: el power
/// arranca en 3 y decrementa un ciclo por vez, así que tres ciclos son +3 de Fuerza y se acabó.
/// La Fuerza multiplica ~20 impactos por turno (§14.1-4): este tope y el de `MeritIsInTheFeetA`
/// son los dos parches que hacen cerrar la auditoría de pico.
/// </summary>
public sealed class ManifestationOfBishamonten() : KagetoraCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const decimal Cycles = 3m;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BishamontenManifestationPower>("Cycles", Cycles)];
    protected override Task OnPlay(PlayerChoiceContext c, CardPlay p) => PowerCmd.Apply<BishamontenManifestationPower>(c, Owner.Creature, Cycles, Owner.Creature, this);
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
