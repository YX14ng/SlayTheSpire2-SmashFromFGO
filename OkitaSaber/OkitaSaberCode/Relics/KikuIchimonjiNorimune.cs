using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Relics;

/// <summary>
/// Kiku-ichimonji Norimune (菊一文字则宗) — su katana, reliquia POCO COMÚN (DESIGN-OKITA §6.2): tus
/// CRÍTICOS hacen +3 de daño. Un crítico = un Ataque que consume *Crítico Listo (el ×2 de FGOCore).
///
/// Implementación por reúso: al iniciar combate aplica un <see cref="SwordGeniusPower"/> de 3 (el mismo
/// power «Genio de la Espada» — su único trabajo es el +Amount aditivo al golpe que va a criticar,
/// antes del ×2). Si además drafteás la carta Genio de la Espada, los bonos SUMAN (Counter), igual que
/// FafnirHeartblood aplica un power para enganchar el listener de pierce sin reimplementarlo.
/// </summary>
public sealed class KikuIchimonjiNorimune : OkitaRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const int CritBonus = 3;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<SwordGeniusPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await PowerCmd.Apply<SwordGeniusPower>(Owner.Creature, CritBonus, Owner.Creature, null);
    }
}
