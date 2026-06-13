using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Cicatriz del Tilo (菩提叶之伤痕 / Linden Scar) — la DEBILIDAD hecha MOTOR.
/// Cada vez que la Hoja de Tilo deja pasar un golpe (el primer golpe que te ALCANZA cada
/// turno IGNORA la Sangre de Dragón — la espalda expuesta), las escamas se ESPESAN donde
/// la herida sangró: +1 Sangre de Dragón y +<see cref="Amount"/> Carga NP.
///
/// Anti-batería (P2/P3): el pierce es estructuralmente ≤1/turno (la reliquia lo capa con su
/// flag _piercedThisTurn y sólo lo cuenta en golpes que REALMENTE alcanzan, amount>0). Por eso
/// este listener dispara como mucho UNA vez por turno — sin loop AFK ni rider por frecuencia.
/// No puede auto-alimentarse: hace falta que un enemigo te pegue y te alcance.
///
/// <see cref="PowerStackType.Counter"/>: una 2ª copia SUMA su Carga NP (Amount), pero la +1
/// SdD por pierce es PLANA (no escala con copias) — para no sobrealimentar el umbral SdD≥3 que
/// afila a Balmung. Amount guarda la Carga NP por pierce (10; up 15), igual que IronWillPower
/// guarda su valor por turno.
/// </summary>
public sealed class LindenScarPower : SiegfriedPower, IDragonScalePierceListener
{
    public const int ScalesPerPierce = 1; // +SdD por pierce (plano, no escala con copias)

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DragonScalesPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public async Task OnScalesPierced(PlayerChoiceContext choiceContext)
    {
        Flash();
        await PowerCmd.Apply<DragonScalesPower>(Owner, ScalesPerPierce, Owner, null);
        await NpCharge.Gain(Owner, Amount, null);
    }
}
