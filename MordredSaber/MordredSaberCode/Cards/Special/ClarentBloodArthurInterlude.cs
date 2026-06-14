using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Cards;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Special;

/// <summary>
/// Clarent Blood Arthur: Interludio (克拉伦特·血染亚瑟：幕间) — la ulti auto-manifestada a 100 NP
/// MIENTRAS estás en forma Relámpago Carmesí (la manda el GaugeFilled de MainFile al ver esa
/// forma permanente). Versión clímax (upgrade del NP, DESIGN-MORDRED §5.4):
///   - 8 de daño ×5 a TODOS; vs Élites/Jefes +2 por golpe;
///   - SOBRECARGA: +1 por golpe por cada 15 de carga sobre 100;
///   - después: +20 NP.
/// Sin la regla del yelmo (ya está desenmascarada permanentemente). Escala con dupes (NpLevels.Scale).
/// </summary>
public sealed class ClarentBloodArthurInterlude() : MordredCard(0, CardType.Attack, CardRarity.Event, TargetType.AllEnemies), IMordredNpCard
{
    public const int ChargeCost = 100;
    public const int Hits = 5;
    private const int OverchargePer = 15;      // +1/golpe por cada 15 sobre el mínimo (más generoso)

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("Hits", Hits),
        new DynamicVar("Authority", 2),
        new DynamicVar("Refund", 20),
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
