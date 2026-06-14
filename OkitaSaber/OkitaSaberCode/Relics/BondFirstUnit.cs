using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Character;
using OkitaSaber.OkitaSaberCode.Extensions;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Lazo de la Primera Unidad (一番队之绊) — el vínculo de Okita sobre el BondRelic de FGOCore
/// (DESIGN-OKITA §6.1). Espina numérica por defecto (Max HP en Nv1/3/6/9, NP inicial en Nv2/5/8)
/// con dos giros propios:
///   - los regalos de Bloqueo default se reemplazan por *Estrellas de Crítico iniciales (su flavor
///     es el motor de estrellas, NO tanquear): Nv4 → +10★, Nv7 → +20★ (y +1 *Aliento extra en Nv7).
///   - el capstone Nv10 «Luchar Hasta el Final» (战至最后): al inicio de cada combate, +1 *Alzarse
///     (GutsPower, revive al 30%) — su deseo al Grial hecho reliquia.
/// El multiplicador global ×1.25 daño/Bloqueo vive en BondPower (lo aplica base.BeforeCombatStartLate).
/// </summary>
[Pool(typeof(OkitaRelicPool))]
public sealed class BondFirstUnit : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    // El arte vive en los recursos de OkitaSaber, no en los de FGOCore (patrón MorganBond/MashBond).
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<AlientoPower>(),
        HoverTipFactory.FromPower<GutsPower>()
    ];

    // Okita no tanquea: los regalos de Bloqueo se reemplazan por Estrellas (+1 Aliento en Nv7).
    protected override int StartingBlock(int lv) => 0;

    private static int StartingStars(int lv) => lv >= 7 ? 20 : lv >= 4 ? 10 : 0;

    private static int StartingBreathBonus(int lv) => lv >= 7 ? 1 : 0;

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();

        var stars = StartingStars(Level);
        if (stars > 0)
        {
            await CritStars.Gain(Owner.Creature, stars, null);
        }

        var breath = StartingBreathBonus(Level);
        if (breath > 0)
        {
            await Aliento.Gain(Owner.Creature, breath, null);
        }
    }

    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<GutsPower>(Owner.Creature, 1, Owner.Creature, null);
    }
}
