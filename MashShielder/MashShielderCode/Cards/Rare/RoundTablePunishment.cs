using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>Castigo de la Mesa Redonda — your Block, delivered to everyone at once.</summary>
public sealed class RoundTablePunishment() : MashShielderCard(3, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = Owner.Creature.Block;
        if (block <= 0) return;

        await DamageCmd.Attack(block).FromCard(this).TargetingAllOpponents(CombatState)
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
