using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Sacrificio de la Reina (王之牺牲) — nuevo común P2 2026-06-25: 8 de daño; pierdes 1 HP;
/// si tienes *Alzarse: +6 de daño. Da cuerpo al eje "Sangre de la Reina" (autodaño) que estaba
/// subdimensionado (HP base 72, pocas cartas que paguen Vida) y lo conecta con el remate de Guts
/// — la Reina que muere y vuelve más fuerte. Patrón Alzarse copiado de TyrantsLance.
/// </summary>
public sealed class QueensSacrifice() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new HpLossVar(1m),
        new DynamicVar("Bonus", 6)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GutsPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<GutsPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.HasPower<GutsPower>())
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }

        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        await CreatureCmdCompatibility.DamageFromCard(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(3m);
    }
}
