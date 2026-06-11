using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;
using ArtoriaCaster.ArtoriaCasterCode.Powers.Forms;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Escapada de Verano — cambiá a tu forma opuesta; al FINAL del turno volvés a la
/// forma anterior (EscapadeReturnPower). Mejorada: al volver ganás 1★. En Avalon
/// no hace nada de forma.
/// </summary>
public sealed class SummerEscapade() : ArtoriaCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ProphecyCasterFormPower>(), HoverTipFactory.FromPower<SummerBerserkerFormPower>(), HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // En Avalon (forma permanente) no hay ida ni vuelta.
        if (Owner.Creature.HasPower<AvalonFormPower>()) return;

        var wasCaster = Owner.Creature.HasPower<ProphecyCasterFormPower>();
        if (wasCaster)
        {
            await FormSwitch.Enter<SummerBerserkerFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await FormSwitch.Enter<ProphecyCasterFormPower>(choiceContext, Owner.Creature, this);
        }

        await PowerCmd.Apply<EscapadeReturnPower>(Owner.Creature, 1m, Owner.Creature, this);
        var ret = Owner.Creature.GetPowerInstances<EscapadeReturnPower>().FirstOrDefault();
        if (ret != null)
        {
            ret.ReturnToCaster = wasCaster;
            ret.StarsOnReturn = DynamicVars["Stars"].IntValue;
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stars"].UpgradeValueBy(1m);
    }
}
