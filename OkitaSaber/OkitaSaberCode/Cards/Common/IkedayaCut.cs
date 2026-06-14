using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Corte de Ikedaya (池田屋之斩) — DESIGN-OKITA §5.2. 2⚡ At: 18 daño; ganás 1 *Tos (al mazo de robo)
/// (up: 23). El 2⚡ = 18-con-downside; el downside es moneda (convertible a NP/★/cura).
/// </summary>
public sealed class IkedayaCut() : OkitaCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(18m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await Tos.ShuffleIntoDraw(Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(5m); // 18 -> 23
}
