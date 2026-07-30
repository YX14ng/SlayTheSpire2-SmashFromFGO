using BaseLib.Abstracts;
using BaseLib.Extensions;
using FGOCore.FGOCoreCode.Compatibility;
using FGOCore.FGOCoreCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace FGOCore.FGOCoreCode;

/// <summary>Base for FGOCore's shared powers; icons load from FGOCore's resources.</summary>
public abstract class FGOCorePower : CustomPowerModel, IFgoDamageHooks, ILegacyDamageHooks
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    public abstract override PowerType Type { get; }

    public abstract override PowerStackType StackType { get; }

#if STS2_BETA
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay) =>
        ModifyDamageAdditiveFgo(target, amount, props, dealer, cardSource, cardPlay);

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) =>
        ModifyDamageMultiplicativeFgo(target, amount, props, dealer, cardSource, cardPlay);

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay) =>
        ModifyDamageCapFgo(target, props, dealer, cardSource, cardPlay);

    public virtual decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource) => ModifyDamageAdditiveFgo(target, amount, props, dealer, cardSource, null);

    public virtual decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource) =>
        ModifyDamageMultiplicativeFgo(target, amount, props, dealer, cardSource, null);

    public virtual decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer,
        CardModel? cardSource) => ModifyDamageCapFgo(target, props, dealer, cardSource, null);
#else
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource) => ModifyDamageAdditiveFgo(target, amount, props, dealer, cardSource, null);

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource) =>
        ModifyDamageMultiplicativeFgo(target, amount, props, dealer, cardSource, null);

    public override decimal ModifyDamageCap(Creature? target, ValueProp props, Creature? dealer,
        CardModel? cardSource) => ModifyDamageCapFgo(target, props, dealer, cardSource, null);
#endif

    public virtual decimal ModifyDamageAdditiveFgo(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 0m;

    public virtual decimal ModifyDamageMultiplicativeFgo(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay) => 1m;

    public virtual decimal ModifyDamageCapFgo(Creature? target, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay) => decimal.MaxValue;
}
