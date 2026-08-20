using MashShielder.MashShielderCode.Cards;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Castigo de la Mesa Redonda — tu muro, entregado a todos a la vez.
///
/// <para>REDESIGN-MASH-V2 §6.3: era el peor infractor del rediseño — 3⚡ por «daño = tu Bloqueo a
/// TODOS» SIN gastarlo, repetible cada turno con el muro intacto. Ahora **Descarga** (baja a 2⚡
/// porque te deja desnuda) y es `Unpowered`: no escala con Fuerza, para que no se combine con
/// KnightsVow/LordCamelot en un doble motor (parche J1-1).</para>
/// </summary>
public sealed class RoundTablePunishment() : MashShielderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IDischargeCard
{
    private const decimal UpgradedConversion = 1.25m;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Percent", 100)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(MashKeywords.Descargar), HoverTipFactory.Static(StaticHoverTip.Block)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var damage = await Descarga.All(choiceContext, Owner.Creature, DynamicVars["Percent"].BaseValue / 100m);
        if (damage <= 0) return;

        Descarga.ShowFloat(Owner.Creature, damage);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).TargetingAllOpponents(CombatState!)
            .Unpowered()
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Percent"].UpgradeValueBy((UpgradedConversion - 1m) * 100m);
    }
}
