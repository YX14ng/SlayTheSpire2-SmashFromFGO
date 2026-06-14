using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// El Gusano del Abismo (深渊之虫 / The Worm of the Abyss) — DESIGN-OBERON §6.4. 2⚡ Ataque: 10 de daño a
/// TODOS; en VORTIGERN, +8 (up 13 / +10). El payoff AoE de la forma clímax (1.440 km de longitud). Lee
/// <c>HasPower&lt;VortigernPower&gt;()</c>. Glow en Vortigern.
/// </summary>
public sealed class TheWormOfTheAbyss() : OberonCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    private const int VortigernBonus = 8;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(10m, ValueProp.Move), new DynamicVar("Bonus", VortigernBonus)];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<VortigernPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var bonus = Owner.Creature.HasPower<VortigernPower>() ? DynamicVars["Bonus"].IntValue : 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).TargetingAllOpponents(Owner.Creature.CombatState)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
