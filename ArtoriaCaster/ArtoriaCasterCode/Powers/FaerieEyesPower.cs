using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Powers;

/// <summary>
/// Ojos Feéricos (妖精眼) — cada golpe enemigo anulado POR COMPLETO: ganás 1★ y
/// Carga NP +<see cref="NpPerTrigger"/>. Implementación DUAL anti-doble-conteo:
/// (a) AfterDamageReceived dispara con los golpes que vanilla marca WasFullyBlocked;
/// (b) IHitAnnulledListener dispara con las anulaciones de Anti-Purga que vanilla NO
/// contó (AntiPurgePower solo notifica cuando NO fue WasFullyBlocked).
/// </summary>
public sealed class FaerieEyesPower : ArtoriaPower, IHitAnnulledListener
{
    public const int StarsPerTrigger = 1;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    /// <summary>Carga NP por golpe anulado (5; 8 con la carta mejorada).</summary>
    public int NpPerTrigger { get; set; } = 5;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || dealer == null || !props.IsPoweredAttack()) return;
        if (!result.WasFullyBlocked) return;
        await Trigger();
    }

    public Task AfterHitAnnulled(Creature attacker) => Trigger();

    private async Task Trigger()
    {
        Flash();
        await Stars.Gain(Owner, StarsPerTrigger, null);
        await NpCharge.Gain(Owner, NpPerTrigger, null);
    }
}
