using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Uncommon;

/// <summary>
/// Embate de la Capitana (队长突击) — DESIGN-OKITA §5.3. 3⚡ At: 26 daño (up: 33). Slot Bludgeon sin
/// condiciones.
/// </summary>
public sealed class CaptainsOnslaught() : OkitaCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(26m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(7m); // 26 -> 33
}
