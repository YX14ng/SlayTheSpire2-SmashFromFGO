using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Sangre de Dragón Ascendente (龙血渐厚, §rares) — el motor de escamas que define un build full-tanque:
/// al inicio de cada turno +1 SdD (up 2), y +2 al jugarla (up 3). Aplica MaturingScalesPower (escalado
/// por-turno) + un pulso inmediato de SdD. Reusa DragonScalesPower.
/// </summary>
public sealed class DragonbloodAscendant() : SiegfriedCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("ScalesPerTurn", 1), new DynamicVar("Entry", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MaturingScalesPower>(Owner.Creature, DynamicVars["ScalesPerTurn"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Entry"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ScalesPerTurn"].UpgradeValueBy(1m);
        DynamicVars["Entry"].UpgradeValueBy(1m);
    }
}
