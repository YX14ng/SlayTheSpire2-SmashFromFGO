using MashShielder.MashShielderCode.Cards;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>
/// Embestida de Camelot — REDESIGN-MASH-V2 §6.2: el Body Slam honesto. ANTES leía tu Bloqueo SIN
/// gastarlo por 2⚡ (defensa y ofensa eran la misma pila gratis, el pecado del reporte de Moopamoop).
/// Ahora **Descarga**: baja a 1⚡ porque cuesta el muro entero. La mejora sube la CONVERSIÓN, no el
/// coste (parche F8: a 0⚡ sería «vaciá el muro gratis»).
/// </summary>
public sealed class CamelotRam() : MashShielderCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IDischargeCard
{
    private const decimal UpgradedConversion = 1.5m;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", 100)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(MashKeywords.Descargar), HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = await Descarga.All(choiceContext, Owner.Creature, DynamicVars["Percent"].BaseValue / 100m);
        if (damage <= 0) return;

        Descarga.ShowFloat(Owner.Creature, damage);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Percent"].UpgradeValueBy((UpgradedConversion - 1m) * 100m);
    }
}
