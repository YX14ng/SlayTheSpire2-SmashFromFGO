using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Cards;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using FGOCore.FGOCoreCode.CardTypes;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Special;

/// <summary>
/// AROUND CALIBURN: Desatado («La Estrella de Esperanza que te Abraza») — la carta-NP
/// auto-manifestada GRATIS al cruzar 100 NP (consistencia con los otros Servants, 2026-06-26;
/// antes era la «ventana» inline de MainFile). 0⚡, Retain + Exhaust, consume TODA la Carga
/// (mín. !ChargeCost!). El soporte clímax de Castoria: este turno tus Ataques pueden CRITICAR en
/// CUALQUIER forma (cobrás las Estrellas que venías acumulando como Caster), ganás !AntiPurge!
/// Anti-Purga y !Stars! *Estrellas de Crítico. SOBRECARGA: +1★ por cada 10 de Carga sobre el mínimo.
/// </summary>
public sealed class AroundCaliburnUnleashed() : ArtoriaCard(0, CardType.Skill, CardRarity.Event, TargetType.Self), IArtoriaNpCard, ICommandTyped
{
    public const int ChargeCost = 100;

    // TAREA D: tipo de NP del juego original (Around Caliburn = Arts) → bonus de ulti del sistema de tipos.
    CommandType ICommandTyped.CommandType => CommandType.Arts;
    public bool IsNoblePhantasm => true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AntiPurge", 1),
        new DynamicVar("Stars", 3),
        new DynamicVar("ChargeCost", ChargeCost),
        new DynamicVar("PerTen", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<AntiPurgePower>(), HoverTipFactory.FromPower<AroundCaliburnWindowPower>()];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var extraStars = (tier - ChargeCost) / 10 * DynamicVars["PerTen"].IntValue;

        // Ventana «Around Caliburn»: este turno podés CRITICAR en cualquier forma (Stars.CanCrit
        // la reconoce). Expira al fin de tu turno.
        await PowerCmd.Apply<AroundCaliburnWindowPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        await PowerCmd.Apply<AntiPurgePower>(choiceContext, Owner.Creature, DynamicVars["AntiPurge"].BaseValue, Owner.Creature, this);
        await Stars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue + extraStars, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["AntiPurge"].UpgradeValueBy(1m);
        DynamicVars["Stars"].UpgradeValueBy(2m);
    }
}
