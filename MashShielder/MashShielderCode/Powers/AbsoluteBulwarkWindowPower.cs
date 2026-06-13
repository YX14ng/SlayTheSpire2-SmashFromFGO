using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Powers;

/// <summary>
/// Baluarte Absoluto — la VENTANA del NP de Mash (modelo de NP nuevo, 2026-06-12).
/// Se abre al cruzar 100 de Carga NP (en vez de generar una carta-ulti gratis): por
/// este turno, tus Ataques infligen daño adicional igual a tu Bloqueo actual — la
/// muralla golpea. Hace que el mazo DEFENSIVO se vuelva ofensivo durante el clímax
/// (el mazo importa MÁS durante el NP, no menos), sin eclipsarlo con un nuke gratis.
/// El medidor NO se consume al abrir; las cartas NP drafteadas (Lord Camelot, etc.)
/// son el clímax que elegís jugar DENTRO de la ventana. Expira al final de tu turno.
/// </summary>
public sealed class AbsoluteBulwarkWindowPower : MashShielderPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (Owner != dealer || !props.IsPoweredAttack()) return 0m;
        return Owner.Block;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
