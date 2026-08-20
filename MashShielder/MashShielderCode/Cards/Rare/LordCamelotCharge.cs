using MashShielder.MashShielderCode.Cards;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Embate de Lord Camelot (罗德·卡美洛之冲撞) — el cierre ofensivo de Mash fuera de la ventana NP.
///
/// <para>REDESIGN-MASH-V2 §6.3: antes leía el Bloqueo SIN gastarlo («la muralla embiste sin bajar la
/// guardia» decía el docstring — ése era exactamente el defecto) y necesitaba un candado de 1/turno
/// para no ser infinita. Ahora **Descarga** el muro entero y convierte ×1.5: es la versión rara de la
/// Embestida. El candado de 1/turno se retira porque el diseño se autolimita — hay un solo muro.
/// <see cref="Powers.LordCamelotChargePower"/> queda INERTE (no se borra: rompería saves).</para>
/// </summary>
public sealed class LordCamelotCharge() : MashShielderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy), IDischargeCard
{
    private const decimal BaseConversion = 1.5m;
    private const decimal UpgradedConversion = 2m;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", (int)(BaseConversion * 100))];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(MashKeywords.Descargar), HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override bool IsPlayable => Owner.Creature.Block > 0;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = await Descarga.All(choiceContext, Owner.Creature, DynamicVars["Percent"].BaseValue / 100m);
        if (damage <= 0) return;

        Descarga.ShowFloat(Owner.Creature, damage);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .Unpowered()
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Percent"].UpgradeValueBy((UpgradedConversion - BaseConversion) * 100m);
    }
}
