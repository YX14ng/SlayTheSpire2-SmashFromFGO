using KagetoraLancer.KagetoraLancerCode.Doctrine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Powers;

public interface IAscensionListener
{
    Task AfterAscendingToKenshin(PlayerChoiceContext context, CardModel source);
}

/// <summary>Base compartida para las dos formas visuales de Kagetora.</summary>
public abstract class KagetoraFormPower : FormPower
{
    public override bool ShouldScaleInMultiplayer => false;
}

/// <summary>Forma inicial de Nagao Kagetora.</summary>
public sealed class NagaoKagetoraFormPower : KagetoraFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/kagetora_frames.tres";
}

/// <summary>Transformación irreversible a Uesugi Kenshin durante el combate.</summary>
public sealed class KenshinFormPower : KagetoraFormPower
{
    public override string FramesPath => $"{MainFile.ResPath}/character/kenshin_frames.tres";
    public override bool IsPermanent => true;
}

/// <summary>Encarnación: cada ciclo prepara, como máximo, una Bendición.</summary>
public sealed class IncarnationPower : KagetoraPower, IDoctrineCycleListener
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public async Task AfterDoctrineCycle(PlayerChoiceContext context, DoctrineAdvance result)
    {
        Flash();
        await BishamontenBlessingPower.Grant(context, Owner, result.CardPlay.Card);
    }
}

/// <summary>
/// Reserva de Bendición. Antes de un Ataque se transforma en un efecto activo atado al CardPlay;
/// así una Bendición creada por el ciclo de ese mismo Ataque queda intacta para el siguiente.
/// </summary>
public sealed class BishamontenBlessingPower : KagetoraPower
{
    public const int PerHitDamage = 2;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public static async Task Grant(PlayerChoiceContext context, Creature owner, CardModel? source)
    {
        if (owner.HasPower<BishamontenBlessingPower>()) return;
        await PowerCmd.Apply<BishamontenBlessingPower>(context, owner, 1m, owner, source);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner || cardPlay.Card.Type != CardType.Attack) return;

        var context = new BlockingPlayerChoiceContext();
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<BishamontenBlessingActivePower>(
            context, Owner, 1m, Owner, cardPlay.Card, silent: true);
        Owner.GetPower<BishamontenBlessingActivePower>()?.Arm(cardPlay);
    }
}

/// <summary>Efecto oculto que aplica +2 a cada impacto de una sola jugada.</summary>
public sealed class BishamontenBlessingActivePower : KagetoraPower
{
    private CardPlay? _activePlay;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;

    internal void Arm(CardPlay cardPlay) => _activePlay = cardPlay;

    public override decimal ModifyDamageAdditiveFgo(
        Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return _activePlay != null &&
               (_activePlay == cardPlay || cardPlay == null) &&
               _activePlay.Card == cardSource
            ? BishamontenBlessingPower.PerHitDamage
            : 0m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_activePlay != cardPlay) return;
        _activePlay = null;
        await PowerCmd.Remove(this);
    }
}

/// <summary>Marcador de compatibilidad para la manifestación del NP.</summary>
public sealed class NpManifestedPower : KagetoraPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;
    protected override bool IsVisibleInternal => false;
}

/// <summary>Preparación local de Overcharge: cada carga suma un nivel de OC al próximo NP.</summary>
public sealed class OverchargePreparationPower : KagetoraPower, INpOverchargePreparation
{
    public const int MaxStacks = 1;
    public const int ExtraTier = 200;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldScaleInMultiplayer => false;

    public static async Task<int> Consume(PlayerChoiceContext context, Creature owner)
    {
        if (owner.GetPower<OverchargePreparationPower>() is not { } power) return 0;
        var extra = (int)power.Amount * ExtraTier;
        await PowerCmd.Remove(power);
        return extra;
    }

    int INpOverchargePreparation.ExtraTier => ExtraTier;

    public async Task ConsumeOverchargePreparation(PlayerChoiceContext context) =>
        await PowerCmd.Remove(this);
}
