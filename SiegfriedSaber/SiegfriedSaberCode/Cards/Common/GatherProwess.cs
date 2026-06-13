using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Reunir Bravura — setup-burst del motor con EXHAUST como cooldown (§3): de un saque acerca
/// el medidor (20 NP) y el umbral SdD≥3 de Balmung (+2 SdD). El Exhaust evita el loop de generación de
/// SdD (un uso/combate). El up sube SOLO el NP (+10); el SdD es FIJO +2.</summary>
public sealed class GatherProwess() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Np", 20), new DynamicVar("Scales", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Np"].UpgradeValueBy(10m);
}
