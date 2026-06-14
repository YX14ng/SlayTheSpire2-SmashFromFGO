using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Cards;
using MordredSaber.MordredSaberCode.Extensions;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Clarent Blood Arthur (manual) (克拉伦特·血染亚瑟·手动) — DESIGN-MORDRED §5.3. 2⚡ At NP: el NP como
/// carta drafteable. Mín 70, consume TODA la carga: 5 de daño ×5 a TODOS; vs Élites/Jefes +2 por golpe;
/// SOBRECARGA +1/golpe por cada 20 sobre el mínimo; después +10 NP (up: 6×5). Glow al poder pagarse.
/// Nicho: disparar la ulti ANTES de cruzar el auto-manifestado a 100 (a 70 ya pega). Como la token
/// «Desatado», desenmascara PRIMERO (la regla de oro lore) y usa NpLevels.Scale + VersusAuthority().
/// </summary>
public sealed class ClarentBloodArthurManual() : MordredCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IMordredNpCard
{
    public const int ChargeCost = 70;
    public const int Hits = 5;
    private const int OverchargePer = 20;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Hits", Hits),
        new DynamicVar("Authority", 2),
        new DynamicVar("Refund", 10),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Regla de oro lore: enmascarada no puede gritar su rebelión → el yelmo cae PRIMERO.
        await Forms.UnmaskForUlt(choiceContext, Owner.Creature, this);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var overcharge = (tier - ChargeCost) / OverchargePer;
        var authority = Owner.Creature.VersusAuthority() ? DynamicVars["Authority"].IntValue : 0;

        var perHit = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + authority);
        for (var i = 0; i < DynamicVars["Hits"].IntValue; i++)
        {
            await DamageCmd.Attack(perHit).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
                .WithHitFx("vfx/vfx_starry_impact")
                .Execute(choiceContext);
        }

        await NpCharge.Gain(Owner.Creature, DynamicVars["Refund"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(1m);
}
