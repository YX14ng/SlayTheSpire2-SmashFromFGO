using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Caballero del Relámpago Rojo A+ (赤雷骑士A+) — DESIGN-MORDRED §5.2. 2⚡ Poder: tus Ataques hacen
/// +2; tus CRÍTICOS hacen +6 ADICIONAL (up +3/+8). El Rank-Up de Estallido de Maná como Poder; el
/// multiplicador real de la forma ofensiva vive ACÁ (§5), no en la pasiva. Aplica
/// <see cref="KnightOfRedLightningPower"/>: Amount = +Ataque (sube con el up); CritBonus es campo
/// settable que la carta fija desde su DynamicVar (no choca con el conteo de stacks). Patrón
/// WeightOfExpectations/MemoryOfTrifas (poder con campo settable).
/// </summary>
public sealed class KnightOfRedLightningAPlus() : MordredCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<KnightOfRedLightningPower>("Attack", 2m), new DynamicVar("CritBonus", 6)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<KnightOfRedLightningPower>(Owner.Creature, DynamicVars["Attack"].BaseValue, Owner.Creature, this);
        if (power != null) power.CritBonus = DynamicVars["CritBonus"].IntValue;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Attack"].UpgradeValueBy(1m);
        DynamicVars["CritBonus"].UpgradeValueBy(2m);
    }
}
