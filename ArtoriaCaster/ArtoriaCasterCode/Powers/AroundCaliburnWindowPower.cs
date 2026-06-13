using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Around Caliburn (誓约胜利之剑) — la VENTANA del NP de Artoria (modelo de NP nuevo,
/// 2026-06-12). Se abre al cruzar 100 de Carga NP EN VEZ de generar una ulti gratis
/// (que eclipsaba a las cartas NP drafteadas — feedback del usuario). Este turno tus
/// Ataques pueden CRITICAR en CUALQUIER forma (incluso Caster): cobrás las Estrellas
/// que venías acumulando como Caster. <see cref="Stars.CanCrit"/> la reconoce.
/// Expira al final de tu turno. Las cartas NP drafteadas (Around Caliburn / Hope Will
/// Camelot) siguen siendo el clímax que elegís jugar.
/// </summary>
public sealed class AroundCaliburnWindowPower : ArtoriaPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side)
        {
            await PowerCmd.Remove(this);
        }
    }
}
