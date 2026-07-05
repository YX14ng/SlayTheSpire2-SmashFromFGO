using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MordredSaber.MordredSaberCode.Powers.Forms;
// Alias: estamos EN el namespace ...Powers, que tiene un sub-namespace «Forms» — sin esto,
// «Forms» resolvería al namespace en vez de a la fachada estática Forms.Forms.
using MordredForms = MordredSaber.MordredSaberCode.Powers.Forms.Forms;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Retorno de Visita Relámpago — marcador OCULTO temporal de «Visita Relámpago» (§5.2): al final
/// de TU turno volvés a la forma anterior (la ventana de 1 turno: arrancarse el yelmo, rugir, volver).
/// <see cref="ReturnToMasked"/> recuerda qué forma restaurar (la que tenías ANTES del switch de la
/// carta). El up de la carta enciende <see cref="StarsOnReturn"/> (al volver, +10★). Se auto-remueve
/// al revertir. Single: no apila (un solo viaje en vuelo a la vez). Sin icono: es infraestructura de turno.
/// </summary>
public sealed class LightningVisitReturnPower : MordredPower
{
    public bool ReturnToMasked;     // true = volvés a Enmascarado; false = volvés a Rebelión
    public int StarsOnReturn;       // 0 base; 10 tras el up (lo fija la carta)

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    public override bool ShouldScaleInMultiplayer => false;

    protected override bool IsVisibleInternal => false;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Owner.IsDead) return;
        Flash();
        // Regreso REAL (audit 2026-07-05): si la forma destino ya esta activa o la forma actual es
        // permanente (Climax), el Enter es un no-op y el +10 del up no corresponde.
        var alreadyThere = ReturnToMasked ? Owner.HasPower<MaskedKnightFormPower>() : Owner.HasPower<RebellionFormPower>();
        var permanentForm = false;
        foreach (var f in Owner.GetPowerInstances<FGOCore.FGOCoreCode.Forms.FormPower>())
        {
            if (f.IsPermanent) { permanentForm = true; break; }
        }
        var willSwitch = !alreadyThere && !permanentForm;
        // source == null: el regreso AUTOMÁTICO no cuenta como «cambio de forma» (no dispara
        // FormShiftedPower ni a los IFormChangeListener como el Estandarte) — sólo el switch de IDA
        // de la carta lo hace. La ida ya marcó la danza; el regreso es gratis de eventos.
        if (ReturnToMasked)
        {
            await MordredForms.Enter<MaskedKnightFormPower>(choiceContext, Owner, null);
        }
        else
        {
            await MordredForms.Enter<RebellionFormPower>(choiceContext, Owner, null);
        }
        if (StarsOnReturn > 0 && willSwitch)
        {
            await CritStars.Gain(Owner, StarsOnReturn, null);
        }
        await PowerCmd.Remove(this);
    }
}
