using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Sangre del Corazón de Fafnir (法夫纳的心头血) — reliquia de JEFE (§8.3). Al iniciar combate aplica un
/// Bautismo de Fafnir oculto: cuando un golpe ATRAVIESA tu Sangre de Dragón (la espalda expuesta te cuesta
/// HP), la herida se bautiza → +20 Carga NP. Estructuralmente 1/turno (el pierce ya es ≤1/turno).
/// Hereda el motor completo de LindenLeaf porque Orobas elimina la starter: conserva Escamas,
/// debilidad de la espalda, recompensa defensiva y el almacén persistente de nivel NP.
/// </summary>
public sealed class FafnirHeartblood : LindenLeaf
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private const int NpPerPierce = 20;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonScalesPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await PowerCmd.Apply<BaptismOfFafnirPower>(new BlockingPlayerChoiceContext(), Owner.Creature, NpPerPierce, Owner.Creature, null);
    }
}
