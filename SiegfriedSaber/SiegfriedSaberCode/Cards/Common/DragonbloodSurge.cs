using FGOCore.FGOCoreCode.DragonScales;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>
/// Oleada de Sangre de Dragón (龙血涌动 / Dragonblood Surge) — el ataque-feeder barato de profundidad
/// (DESIGN-REVIEW-2). 1⚡ At: 6 de daño + 1 de Sangre de Dragón + 5 de Carga NP. El triple feeder ligero:
/// pega, espesa la piel y carga el medidor a la vez — el "motor en una carta" que el pool corto no tenía
/// fuera de los Basic. Números modestos a propósito (no pisa a HeavyBlow 2⚡/16 ni a los Buster grandes).
/// Patrón HeavyBlow (daño + Apply SdD) con un rider de NP. El up sube el daño base.
/// </summary>
public sealed class DragonbloodSurge() : SiegfriedCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Scales", 1), new DynamicVar("Np", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<DragonScalesPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<DragonScalesPower>(choiceContext, Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
