using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Base LOCAL de los powers que suman +<see cref="PowerModel.Amount"/> ADITIVO al daño de tus
/// Ataques (Postura Veloz, Filo de Gloria, Genio de la Espada). Factoriza el guard idéntico de
/// <see cref="ModifyDamageAdditive"/> (sólo Ataques con poder del propio owner) y deja a cada
/// subclase decidir CUÁNDO se aplica el bono vía <see cref="BonusApplies"/> (Postura Veloz: siempre;
/// Filo de Gloria / Genio: sólo cuando hay un *Crítico Listo en cola). La auto-remoción al fin de
/// turno y los hover tips quedan en las subclases porque divergen.
/// Counter, personal: no escala en multijugador.
/// </summary>
public abstract class AttackDamageAdditivePower : OkitaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>¿Se aplica el bono al golpe actual? (Postura Veloz: siempre; los crit-additive lo
    /// gatean con un *Crítico Listo en cola.)</summary>
    protected virtual bool BonusApplies() => true;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack() || cardSource == null) return 0m;
        return BonusApplies() ? Amount : 0m;
    }
}
