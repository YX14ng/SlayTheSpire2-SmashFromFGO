using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Basic;

/// <summary>Golpe — ataque básico Buster (un tajo de Balmung). Lleva el tag Strike (P6 Morgan).
/// Es la carta de comando Buster del mazo inicial QAABB: ICommandTyped(Buster) → bonus de Fuerza temporal.</summary>
public sealed class Strike() : GilgameshCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), ICommandTyped
{
    protected override HashSet<CardTag> CanonicalTags => new() { CardTag.Strike };

    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue, "vfx/vfx_attack_blunt");
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
