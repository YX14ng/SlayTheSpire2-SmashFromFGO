using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Carga de la Cacería (狂猎冲锋) — rediseño v2, patrón 黑闪/"Black" de Jeanne (la
/// carta que dispara el umbral y se cobra a sí misma): obtené 100 Estrellas de
/// Crítico (cruza el umbral → CRÍTICO LISTO automático) y LUEGO infligí 10 de daño
/// a TODOS. El propio ataque reclama el ×2 — CritReadyPower es por CARTA, así que
/// toda la AoE crítica. La Cacería Salvaje cabalga una lluvia de estrellas. (up +4)
/// </summary>
public sealed class WildHuntCharge() : MorganCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("Stars", 100)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Estrellas ANTES del golpe. Morgan BANCA estrellas (IBanksCritStars vía su forma),
        // así que el auto-proc a 100 NO corre para ella: hay que disparar el Crítico a mano.
        // Crit.TrySpend gasta 50★ y aplica CRÍTICO LISTO a ESTA carta (CritReady es por carta,
        // así que toda la AoE critica) antes del golpe. Gana 100, gasta 50 → +50 neto al banco.
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await Crit.TrySpend(Owner.Creature, this);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this)
            .TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_starry_impact")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}
