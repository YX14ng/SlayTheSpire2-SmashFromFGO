using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Common;

/// <summary>Fauces de la Larva — el ataque pan-y-manteca que el pool NO tenía (regla del 40% de
/// sts2-mechanics-design §4.5): 9 daño puro a tasa común exacta (baseline 9-10/1⚡), sin rider.
/// Todos los demás ataques de Lily pagan parte de su tasa en sembrar/parir; este solo PEGA.
/// Además cierra el hueco Común/Ataque=2 que rompía el evento de pociones (CreateForReward pide
/// 3 distintas por rareza×tipo).</summary>
public sealed class LarvalMaw() : TiamatCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_pierce")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
