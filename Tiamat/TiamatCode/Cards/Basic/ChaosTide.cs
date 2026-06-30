using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace TiamatBeast.TiamatCode.Cards.Basic;

/// <summary>Marea de Caos — básica HÍBRIDA (el lodo negro del Mar de Vida): golpe a tasa reducida
/// que SIEMBRA el puente de Maldición desde el turno 1. Lily acuña la divisa que la Bestia gasta.</summary>
public sealed class ChaosTide() : TiamatCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    // Tag Strike vanilla: es el Ataque básico de Tiamat (su "Golpe"). Sin él, contenido base que asume
    // que todo personaje tiene un Basic+Strike CRASHEA — p.ej. LargeCapsule (巨大扭蛋) hace
    // CardPool.AllCards.First(Basic && Strike) y tira InvalidOperationException al obtenerse. Además
    // habilita sinergias/eventos que cuentan "Golpes". (Mismo fix que StrikeMorgan.)
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Curse", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);
        await Curses.Apply(cardPlay.Target, DynamicVars["Curse"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Curse"].UpgradeValueBy(1m);
    }
}
