using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Common;

/// <summary>Golpe Pesado — payoff de tempo: golpe gordo que TAMBIÉN engrosa las escamas hacia SdD≥3.
/// SKILL §2: ataque 2⚡ ≈ 16-18; 16 + rider +2 SdD persistente (8/⚡, bajo el 9-10/⚡ puro, paga el rider).
/// El coste 2⚡ limita la frecuencia. El SdD es FIJO +2 (no escala con up; anti-apilado).</summary>
public sealed class HeavyBlow() : SiegfriedCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(16m, ValueProp.Move), new DynamicVar("Scales", 2)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
