using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Cards.Basic;

public sealed class Buster() : KagetoraCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(10m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab").Execute(context);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Arts() : KagetoraCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, Precept.Heaven), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("NpCharge", 30)];

    // Canal 4 (§4.4): los tooltips PROPIOS van en PreceptHoverTips; sobrescribir ExtraHoverTips
    // reemplazaría la lista de la base y esta carta perdería el tooltip de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(context);
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

/// <summary>
/// Quick básica. §6 la especifica como «6 daño, +30★» (regla 4.6.1) y el código YA cumple: son
/// **20 impresas + 10 del bonus universal de Quick** (`CommandBonusPower.QuickStars`, que corre tras
/// resolver). No subir el var a 30: duplicaría el caudal de estrellas contra el que se calibró E4
/// (un ciclo = 50★ = exactamente un crítico).
/// </summary>
public sealed class Quick() : KagetoraCard(
    1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy, Precept.Feet), ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Quick;
    public bool IsNoblePhantasm => false;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Stars", 20)];

    // Canal 4 (§4.4): tooltip propio en PreceptHoverTips, para conservar el de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_starry_impact").Execute(context);
        await CritStars.Gain(context, Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}

public sealed class Defender() : KagetoraCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.Self, Precept.Chest)
{
    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(5m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay) =>
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}

public sealed class FortuneIsInHeaven() : KagetoraCard(
    1, CardType.Skill, CardRarity.Basic, TargetType.Self, Precept.Heaven)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 20), new CardsVar(1)];

    // Canal 4 (§4.4): tooltip propio en PreceptHoverTips, para conservar el de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}

/// <summary>
/// Encarnación de Bishamonten — el ÚNICO cambio del mazo inicial en el rediseño (§6).
///
/// **1⚡ → 0⚡.** A 1⚡ era una trampa medida: es de precepto NEUTRAL, así que el turno que la jugabas
/// quedabas con 2⚡ y el ciclo (Cielo→Pecho→Pies, 3 cartas) era imposible — había que sacrificar un
/// ciclo entero para encender el poder cuya única función es recompensar ciclos (bug P3 de §11.3).
/// A 0⚡ se juega ENCIMA del ciclo, y así cierra la cuenta del turno 1 de §6.1 (5 cartas jugadas).
///
/// **E5 no aplica:** la regla «todo 0⚡ tiene que gastar un recurso» habla de 0⚡ REPETIBLES. Esta es
/// una carta de Poder del mazo inicial: se juega una vez por combate y se queda. Por eso tampoco
/// lleva Agotar (perdería el poder).
///
/// **La mejora deja de ser <c>EnergyCost.UpgradeBy(-1)</c>** (a coste 0 era un no-op) y pasa a
/// +20 de Carga NP al jugarla, siguiendo el patrón de mejora-que-agrega-efecto del repo
/// (`MorganBerserker.WinterCourt`): el var es constante y el gate es <c>IsUpgraded</c>, así el texto
/// de la mejora vive en el span `+…+` de la loc.
///
/// *Declarado en §6:* es la firma <see cref="BaseLib.Abstracts.ITranscendenceCard"/> del mazo; a 0⚡
/// pierde la rebaja de coste de Infinite Upgrades. Aceptado por el panel.
/// </summary>
public sealed class IncarnationOfBishamonten() : KagetoraCard(
    0, CardType.Power, CardRarity.Basic, TargetType.Self), BaseLib.Abstracts.ITranscendenceCard
{
    public MegaCrit.Sts2.Core.Models.CardModel GetTranscendenceTransformedCard() =>
        MegaCrit.Sts2.Core.Models.ModelDb.Card<Rare.ManifestationOfBishamonten>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<IncarnationPower>("Incarnation", 1m), new DynamicVar("NpCharge", 20)];

    // Canal 4 (§4.4): tooltips propios en PreceptHoverTips, para conservar el de la Doctrina.
    protected override IEnumerable<IHoverTip> PreceptHoverTips =>
    [
        HoverTipFactory.FromPower<IncarnationPower>(),
        HoverTipFactory.FromPower<BishamontenBlessingPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext context, CardPlay cardPlay)
    {
        await PowerCmd.Apply<IncarnationPower>(context, Owner.Creature, 1m, Owner.Creature, this);

        // Mejora (§6): +20 de Carga NP al jugarla, en vez de la rebaja de coste ya imposible.
        if (IsUpgraded)
        {
            await NpCharge.Gain(context, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    // La mejora no toca números: agrega el efecto de arriba. Ver el resumen de la clase.
    protected override void OnUpgrade()
    {
    }
}
