using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Powers.Forms;

/// <summary>
/// Base de las tres formas de Mordred sobre el FormPower genérico de FGOCore (los íconos viven
/// en los recursos de MordredSaber). Cada pasiva es un valor virtual para que el clímax
/// (Relámpago Carmesí) combine lo bueno de las dos sin penalización (espejo de MashFormPower):
///   - <see cref="AttackDamageDelta"/>: tus Ataques hacen ±N (Enmascarado −2, Rebelión/Clímax +2).
///   - <see cref="TakesExtraDamage"/>: recibís +2 por golpe enemigo (solo Rebelión, el all-in).
///   - <see cref="BlockRetentionCap"/>: Bloqueo conservado al final del turno (Enmascarado/Clímax 10).
///   - <see cref="NpPerTurnStart"/>: Carga NP al inicio de tu turno (Enmascarado/Clímax 5).
/// El daño extra recibido en Rebelión es, vía la starter, +10★ por golpe (el odio se recicla).
/// </summary>
public abstract class MordredFormPower : FormPower, IBlockRetentionSource
{
    public const int AttackBonus = 2;          // ±2 a tus Ataques (DESIGN-MORDRED §3.bis)
    public const int DamageTakenPenalty = 2;   // +2 recibido por golpe en Rebelión (la tasa Berserker)
    public const int MaskedRetention = 10;     // Baluarte de Enmascarado/Clímax
    public const int NpPerTurn = 5;            // +5 NP/turno de Enmascarado/Clímax

    // Íconos en los recursos de MordredSaber, no los de FGOCore.
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

    /// <summary>Delta a TUS Ataques (negativo en Enmascarado, positivo en Rebelión/Clímax).</summary>
    protected virtual int AttackDamageDelta => 0;

    /// <summary>Recibís +2 de daño por golpe enemigo (la tasa Berserker de Rebelión).</summary>
    protected virtual bool TakesExtraDamage => false;

    /// <summary>Bloqueo que conservás al final del turno (0 = nada).</summary>
    protected virtual int BlockRetentionCap => 0;

    /// <summary>Carga NP al inicio de tu turno.</summary>
    protected virtual int NpPerTurnStart => 0;

    public decimal RetentionCap(Creature creature) => creature == Owner ? BlockRetentionCap : 0m;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner.Player || Owner.Player == null || Owner.IsDead) return;
        if (NpPerTurnStart <= 0) return;
        Flash();
        await NpCharge.Gain(Owner, NpPerTurnStart, null);
    }

    // ModifyDamageAdditive es DELTA (default 0). Cubre las dos direcciones (espejo SummerBerserker).
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack()) return 0m;
        if (dealer == Owner) return AttackDamageDelta;
        if (TakesExtraDamage && target == Owner && dealer != null && dealer != Owner) return DamageTakenPenalty;
        return 0m;
    }
}
