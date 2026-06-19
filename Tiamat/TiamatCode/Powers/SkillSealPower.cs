using System.Collections.Generic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Sello de Habilidad (スキル封印) — debuff que Tiamat aplica a los enemigos al abrir la ventana
/// Génesis (NP <c>Nammu Dur-an-ki</c>) y vía algunas cartas Lily/Bestia (Sello de las Mareas).
///
/// PLACEHOLDER (ver REDESIGN-TIAMAT.md): v0.107.1 no expone un "no puede usar habilidades / no
/// puede aplicar buffs/debuffs" nativo que se pueda colgar limpiamente desde un PowerModel del
/// jugador, así que de momento es un <b>marcador-Counter</b> que decae 1 por turno del enemigo
/// sellado. Otros powers/relics pueden consultarlo (<c>creature.HasPower&lt;SkillSealPower&gt;()</c>)
/// para condicionar su lógica; el efecto "duro" (cancelar la intención de habilidad del enemigo)
/// queda pendiente de una API de intenciones del juego — DOCUMENTADO como tal para que el lead lo
/// cierre cuando se confirme el hook. Espeja el timing/decremento de <c>CursePower</c>.
/// </summary>
public sealed class SkillSealPower : TiamatPower
{
    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    /// <summary>Ofensa personal: no escala en multijugador (espeja CursePower).</summary>
    public override bool ShouldScaleInMultiplayer => false;

    /// <summary>Decae 1 al inicio del turno del lado del enemigo sellado (mismo timing que la Maldición).</summary>
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        Flash();
        await PowerCmd.Decrement(this);
    }
}
