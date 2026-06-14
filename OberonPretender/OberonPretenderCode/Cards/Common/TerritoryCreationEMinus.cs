using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OberonPretender.OberonPretenderCode.Cards.Common;

/// <summary>
/// Creación de Territorio E− (阵地作成E− / Territory Creation E−) — DESIGN-OBERON §6.2 (el meme
/// permitido, semi-neutral). 0⚡ Habilidad: 2 de Bloqueo. «El rango más bajo del juego: rey sólo de
/// nombre.» El up sube el Bloqueo a 4 y agrega +5 Carga NP (el chiste se vuelve útil al mejorarlo).
/// </summary>
public sealed class TerritoryCreationEMinus() : OberonCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(2m, ValueProp.Move), new DynamicVar("Np", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (DynamicVars["Np"].IntValue > 0)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Np"].UpgradeValueBy(5m);
    }
}
