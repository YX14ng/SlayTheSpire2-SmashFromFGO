using MashShielder.MashShielderCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Aplastamiento — rediseño v2: 1E Ataque; consume hasta 10 de tu Bloqueo (up 15)
/// e inflige 8 daño (up 11) +1 por punto consumido. De anti-sinergia («pierde 4
/// Bloqueo») a gasto VOLUNTARIO de Bloqueo como munición — combea con Ortinax.
/// </summary>
public sealed class Crush() : MashShielderCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("MaxConsume", 10)
    ];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.Block > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var consumed = await Owner.Creature.ConsumeBlockUpTo(DynamicVars["MaxConsume"].IntValue);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + consumed).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["MaxConsume"].UpgradeValueBy(5m);
    }
}
