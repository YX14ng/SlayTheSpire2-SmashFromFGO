using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Medicina del Dr. Matsumoto (松本医生之药) — reliquia POCO COMÚN (DESIGN-OKITA §6.2): la PRIMERA *Tos
/// que tenés cada combate se "trata" (exhausta) y te da +<see cref="NpGain"/> Carga NP. La medicina del
/// médico personal de Okita convierte el primer ataque de tos en combustible.
///
/// Detección estructural 1/combate (flag de código, no "vigilar"): al inicio de cada turno revisa si hay
/// una Tos en tus pilas (robo/mano/descarte) y, si nunca disparó este combate, paga el +NP.
///
/// NOTA DE FASE (alcance "reliquias"): el EXHAUST físico de una carta concreta no tiene API verificada en
/// los proyectos de referencia (SiegfriedSaber/FGOCore sólo exponen Draw/Add/Discard de pila). Para no
/// inventar API (regla dura), hoy el efecto seguro y fiel es el +NP de un saque sobre la primera Tos (la
/// cura mecánica), con la misma detección estructural de DasRheingold (flag _firedThisCombat). El exhaust
/// se enganchará cuando la fase de cartas exponga el helper de retiro de carta de la propia Tos.
/// </summary>
public sealed class DrMatsumotoMedicine : OkitaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int NpGain = 20;

    private bool _firedThisCombat;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    public override Task BeforeCombatStartLate()
    {
        _firedThisCombat = false;
        return base.BeforeCombatStartLate();
    }

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Creature.Side || _firedThisCombat) return;
        if (!HasTos(combatState)) return;
        _firedThisCombat = true;
        Flash();
        await NpCharge.Gain(Owner.Creature, NpGain, null);
    }

    private bool HasTos(CombatState combatState)
    {
        var player = Owner.Creature.Player;
        if (player == null) return false;
        foreach (var pile in new[] { PileType.Hand, PileType.Draw, PileType.Discard })
        {
            foreach (var card in CardPile.GetCards(player, pile))
            {
                if (card is Tos) return true;
            }
        }
        return false;
    }
}
