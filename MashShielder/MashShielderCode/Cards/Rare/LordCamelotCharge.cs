using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Embate de Lord Camelot (罗德·卡美洛之冲撞 / Lord Camelot Charge) — nueva rara P2 2026-06-25: el cierre
/// ofensivo que le faltaba a Mash en acto 3 FUERA de la ventana NP. 2⚡ Ataque: inflige daño igual a tu
/// Baluarte (Bloqueo) actual ×1, *Unpowered (NO escala con Fuerza), 1 vez por turno. No consume el
/// Bloqueo: la muralla embiste sin bajar la guardia. Su daño
/// = Owner.Block) pero como carta de un disparo, gateada por <see cref="LordCamelotChargePower"/>.
/// El daño = Block sin Fuerza mantiene el techo controlado (un mazo Baluarte alto ya es el escalado).
/// </summary>
public sealed class LordCamelotCharge() : MashShielderCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    private static bool CanFire(MegaCrit.Sts2.Core.Entities.Creatures.Creature creature)
    {
        var gate = creature.GetPowerInstances<LordCamelotChargePower>().FirstOrDefault();
        return gate == null || gate.CanFire;
    }

    protected override bool IsPlayable => CanFire(Owner.Creature) && Owner.Creature.Block > 0;

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Asegura el control 1/turno y lo marca como disparado (idempotente: Single).
        var gate = await PowerCmd.Apply<LordCamelotChargePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (gate != null) await gate.MarkFired(choiceContext, this);

        await DamageCmd.Attack(Owner.Creature.Block).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .Unpowered()
            .WithHitFx("vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
