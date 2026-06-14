using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Corte Cruzado (十字斩) — DESIGN-OKITA §5.3. 1⚡ At: 8 daño; si YA jugaste otro Ataque este turno:
/// +10★ y +10 Carga NP (up: 11). Rider Shukuchi (injerto B). Lee <see cref="AttacksThisTurnPower"/>
/// (>0 ⇒ ya jugaste un Ataque). Glow cuando se cumple.
/// </summary>
public sealed class CrossCut() : OkitaCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(8m, ValueProp.Move), new DynamicVar("Stars", 10), new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    private bool AlreadyAttacked => AttacksThisTurnPower.PlayedBefore(Owner.Creature) > 0;

    protected override bool ShouldGlowGoldInternal => AlreadyAttacked;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var already = AlreadyAttacked;
        await AttacksThisTurnPower.EnsureInstalled(Owner.Creature);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        if (already)
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m); // 8 -> 11
}
