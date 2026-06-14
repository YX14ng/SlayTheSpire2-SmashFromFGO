using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Powers.Forms;
using FormsHelper = MordredSaber.MordredSaberCode.Powers.Forms.Forms;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Doble Filo del Odio (憎恶双刃, §5.3) — stack del arquetipo de forma ofensiva (redundante adrede):
/// MIENTRAS estás en Rebelión (o Clímax), tus Ataques hacen +<see cref="AttackBonus"/> ADICIONAL
/// (encima del +2 de la forma). Apaga su buff fuera de la forma ofensiva. El <see cref="AttackBonus"/>
/// es campo settable que fija la carta desde su DynamicVar (up 3→4); Amount es el conteo de stacks
/// (Counter). Patrón espejo de KnightOfRedLightningPower (+Ataque condicional). Personal.
/// </summary>
public sealed class DoubleEdgeOfHatredPower : MordredPower
{
    public int AttackBonus = 3; // up 4 (la carta lo setea desde su DynamicVar)

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<RebellionFormPower>()];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner || !props.IsPoweredAttack()) return 0m;
        return FormsHelper.InOffensiveForm(Owner) ? AttackBonus * (int)Amount : 0m;
    }
}
