using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Balmung: Desatado (幻想大剑·天魔失坠) — la carta-NP de Siegfried. AoE Buster que consume TODA
/// la Carga NP (mín. 100, la manifestación). Overcharge FIEL a la armadura (DESIGN-SIEGFRIED §4):
/// +1 daño por cada 10 de carga sobre el mínimo, o +2 EN SU LUGAR mientras tu Sangre de Dragón ≥ 3
/// — "la sangre del dragón afila el filo" (él ES el dragón); el escalado pleno se GANA aguantando
/// la armadura, sin Marca ni segundo contador.
///
/// El refund +20 NP NO se hornea en la base: se gana con el Rank-Up (A+ → EX, §6) junto con el
/// flag anti-doble-refund (P5). Esta es la base (sin refund).
/// </summary>
public sealed class Balmung() : SiegfriedCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), ISiegfriedNpCard
{
    // Mínimo = la manifestación a 100 (NP a full, fiel a FGO).
    public const int ChargeCost = 100;

    private const int OverchargePerTen = 1;       // +1 daño por cada 10 sobre el mínimo
    private const int OverchargePerTenScaled = 2; // +2 en su lugar si SdD >= 3
    private const int ScaledThreshold = 3;        // SdD a partir del cual el filo se afila

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<DragonScalesPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost);
    // Resplandor dorado cuando es jugable Y la armadura afila el filo (SdD >= 3).
    protected override bool ShouldGlowGoldInternal => IsPlayable && GlowAtScales(ScaledThreshold);

    private const int RefundFull = 20;     // refund EX la 1ª carta-NP del turno
    private const int RefundReduced = 10;  // refund EX si ya resolvió una carta-NP este turno (P5)

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // P5: capturá si YA resolvió una carta-NP este turno ANTES de consumir (ConsumeAllForNpCard
        // setea el flag) — el refund EX paga full la 1ª ult del turno, reducido en las siguientes.
        var alreadyResolved = NpCharge.WasNpResolvedThisTurn(Owner.Creature);

        var tier = await NpCharge.ConsumeAllForNpCard(Owner.Creature, ChargeCost, this);
        var scaled = Scales >= ScaledThreshold;
        var perTen = scaled ? OverchargePerTenScaled : OverchargePerTen;
        var overcharge = (tier - ChargeCost) / 10 * perTen;

        var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge);
        await DamageCmd.Attack(damage).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState!)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);

        // Rank-Up A+ → EX (§6): el refund +20 NP es el único rider plano, y SÓLO en la EX.
        if (IsUpgraded)
        {
            await NpCharge.RefundAfterNpCard(Owner.Creature, RefundFull, RefundReduced, alreadyResolved, this);
        }
    }

    // Rank-Up A+ → EX (§6): el up sube el daño del NP y habilita el refund +20 (arriba, en OnPlay).
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(10m);
}
