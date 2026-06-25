using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MorganBerserker.MorganBerserkerCode.Extensions;

namespace MorganBerserker.MorganBerserkerCode.Powers;

/// <summary>
/// Sentencia de la Reina — la VENTANA del NP de Morgan (rediseño 2026-06-15: swap
/// Estrellas→Maldición). Se abre al cruzar 100 de Carga NP EN VEZ de generar una ulti gratis.
/// Al abrirse (en <see cref="MainFile"/>) detona de una a TODOS los enemigos malditos por su
/// Maldición SIN consumirla (la cosecha masiva de la Reina; el campo sigue sembrado para tus
/// Ataques de Sentencia). Este power es el MARCADOR de la ventana de 1 turno (sabor + tooltip);
/// expira al final de tu turno. Las cartas NP drafteadas (Roadless Camelot, etc.) siguen siendo
/// el clímax que elegís jugar dentro. Patrón calcado de AbsoluteBulwarkWindowPower (Mash).
/// </summary>
public sealed class QueensSentenceWindowPower : MorganPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    // Reusa el arte del marcador NP existente (no hay icono propio aún; sin esto cae al power.png genérico).
    public override string CustomPackedIconPath => "np_manifested_power.png".PowerImagePath();
    public override string CustomBigIconPath => "np_manifested_power.png".BigPowerImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
