using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

/// <summary>
/// Festival de Verano (Berserker, verano 2023) — the cash-out window.
/// (a) Your Attacks deal +2 damage and your cards can trigger CRITICAL.
/// (b) You take +2 damage from each enemy attack (the Berserker class tax —
///     structural anti-parking that scales with enemy pressure).
/// </summary>
public sealed class SummerBerserkerFormPower : ArtoriaFormPower
{
    public const int AttackBonus = 2;
    public const int DamageTakenPenalty = 2;

    public override string FramesPath => $"{MainFile.ResPath}/character/artoria_frames_berserker.tres";

    // ModifyDamageAdditive es DELTA (default 0).
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!props.IsPoweredAttack()) return 0m;
        if (dealer == Owner) return AttackBonus;
        if (target == Owner && dealer != null && dealer != Owner) return DamageTakenPenalty;
        return 0m;
    }
}
