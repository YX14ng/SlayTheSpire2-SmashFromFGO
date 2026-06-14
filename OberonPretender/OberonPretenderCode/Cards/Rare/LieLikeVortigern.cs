using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Forms;
using OberonPretender.OberonPretenderCode.Powers;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Rare;

/// <summary>
/// Lie Like Vortigern (谎言如沃提庚 / Lie Like Vortigern) — DESIGN-OBERON §6.4, el NP verdadero solo-lore
/// como carta-clímax. 2⚡ Poder · Exhaust: consumí TODA tu Deuda — 3 de daño a TODOS por punto consumido;
/// entrás en VORTIGERN (permanente). Patrón Coronación de Invierno (FormSwitch.Enter con source = la
/// carta → dispara los listeners de cambio de forma; el AoE se reparte iterando los enemigos vivos). El
/// up baja el coste a 1⚡.
/// </summary>
public sealed class LieLikeVortigern() : OberonCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    private const int DamagePerDebt = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("PerDebt", DamagePerDebt)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DebtPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var debt = DebtPower.Of(Owner.Creature);
        var consumed = await DebtPower.Forgive(Owner.Creature, debt);
        if (consumed > 0)
        {
            var aoe = consumed * DynamicVars["PerDebt"].IntValue;
            foreach (var enemy in Owner.Creature.CombatState.GetOpponentsOf(Owner.Creature).ToList())
            {
                if (enemy.IsDead) continue;
                await DamageCmd.Attack(aoe).FromCard(this).Targeting(enemy)
                    .WithHitFx("vfx/vfx_starry_impact")
                    .Execute(choiceContext);
            }
        }
        await FormSwitch.Enter<VortigernPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
