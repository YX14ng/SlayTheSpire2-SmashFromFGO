using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Cards;
using MordredSaber.MordredSaberCode.Extensions;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Special;

/// <summary>
/// Clarent Blood Arthur: Desatado (克拉伦特·血染亚瑟：解放) — la ulti auto-manifestada a 100 NP
/// (Retain, Exhaust, gratis). Mín 100, consume TODA la carga. DESIGN-MORDRED §5.4:
///   - REGLA DE ORO LORE: si estás Enmascarada, PRIMERO te arrancás el yelmo (entrás en Rebelión)
///     — así el +2 de Rebelión cae sobre los 5 golpes;
///   - 6 de daño ×5 a TODOS; vs Élites/Jefes +1 por golpe;
///   - SOBRECARGA: +1 por golpe por cada 20 de carga sobre 100;
///   - después: +10 NP.
/// Escala +15%/nivel de NP (NpLevels.Scale, por golpe). El +2 de forma se aplica solo (la forma
/// modifica todo Ataque potenciado). Patrón MemoryOfLondiniumUnleashed.
/// </summary>
public sealed class ClarentBloodArthurUnleashed() : MordredCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IMordredNpCard
{
    public const int ChargeCost = 100;
    public const int Hits = 5;
    private const int OverchargePer = 20;      // +1/golpe por cada 20 sobre el mínimo

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Hits", Hits),
        new DynamicVar("Authority", 1),
        new DynamicVar("Refund", 10),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Regla de oro lore: enmascarada no puede gritar su rebelión → el yelmo cae PRIMERO.
        await Forms.UnmaskForUlt(choiceContext, Owner.Creature, this);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / OverchargePer;
        var authority = Owner.Creature.VersusAuthority() ? DynamicVars["Authority"].BaseValue : 0;

        var perHit = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + authority);
        for (var i = 0; i < DynamicVars["Hits"].IntValue; i++)
        {
            await DamageCmd.Attack(perHit).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
                .WithHitFx("vfx/vfx_starry_impact")
                .Execute(choiceContext);
        }

        await NpCharge.Gain(Owner.Creature, DynamicVars["Refund"].IntValue, this);
    }
}
