using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using TiamatBeast.TiamatCode.Powers;
using TiamatBeast.TiamatCode.Powers.Seal;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>Trampa de Resaca — control CON cuerpo (DESIGN-REVIEW-2 §2): pega y SELLA la habilidad del
/// enemigo (cancela su próxima acción que no sea ataque). El Sello de Habilidad real, accesible en
/// Lily fuera de la ventana Génesis y pegado a un golpe — niega la técnica mientras le hacés daño.</summary>
public sealed class RiptideSnare() : TiamatCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Seal", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<SkillSealPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        if (cardPlay.Target.IsAlive)
        {
            await Sello.Apply(choiceContext, cardPlay.Target, DynamicVars["Seal"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
