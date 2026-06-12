using FGOCore.FGOCoreCode.Stars;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Análisis de Combate — NUEVA del rediseño v2, reemplaza a ChaldeaLibrary
/// (hereda su slot de arte: biblioteca → sala de análisis de Chaldea).
/// 1E Poder: al inicio de tu turno +10 Estrellas de Crítico; cada vez que obtienes
/// CRÍTICO LISTO: roba 1 (up: +20 Estrellas/turno). El robo genérico isla pasa a ser
/// el motor de estrellas.</summary>
public sealed class CombatAnalysis() : MashShielderCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<CombatAnalysisPower>("CombatAnalysis", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CombatAnalysisPower>(Owner.Creature, DynamicVars["CombatAnalysis"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CombatAnalysis"].UpgradeValueBy(10m);
    }
}
