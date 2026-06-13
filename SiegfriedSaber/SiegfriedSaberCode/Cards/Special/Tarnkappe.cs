using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

/// <summary>
/// Tarnkappe / Manto de las Sombras (隐身斗篷) — Ancient. 2⚡ Exhaust: +3 Sangre de Dragón y, el resto del
/// combate, tu espalda deja de estar expuesta (TarnkappePower): el primer golpe que te alcanza se reduce con
/// la SdD como cualquier otro, las escamas dejan de traspasarse y la Hoja de Tilo no te da NP por golpes que
/// alcanzan. El antitanque hecho seguridad — a costa de apagar el motor de NP de la espalda expuesta.
/// </summary>
public sealed class Tarnkappe() : SiegfriedCard(2, CardType.Power, CardRarity.Ancient, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Scales", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<TarnkappePower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Scales"].UpgradeValueBy(2m);
}
