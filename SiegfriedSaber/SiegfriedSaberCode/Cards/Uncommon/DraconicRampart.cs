using FGOCore.FGOCoreCode.Block;
using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Bastión Dracónico (龙鳞壁垒 / Draconic Rampart) — la carta de RETENCIÓN de Baluarte temática
/// (DESIGN-REVIEW-2: la profundidad pedía una retención sobre el motor SdD). 1⚡ Hab: 10 de Bloqueo
/// *Baluarte (persiste al próximo turno, como la piel del dragón que no se cae) + 2 de Sangre de
/// Dragón. El doble eje defensivo de Siegfried: el Baluarte tanquea AHORA y las escamas reducen los
/// golpes futuros. Patrón StrategicWithdrawal (BlockRetention.GainBulwarkBlock) sin el cleanse — más
/// barata y repetible (sin Exhaust), pensada para el muro turno-a-turno y no para la emergencia.
/// </summary>
public sealed class DraconicRampart() : SiegfriedCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new BlockVar(10m, ValueProp.Move), new DynamicVar("Scales", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue);
        await PowerCmd.Apply<DragonScalesPower>(choiceContext, Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(4m);
}
