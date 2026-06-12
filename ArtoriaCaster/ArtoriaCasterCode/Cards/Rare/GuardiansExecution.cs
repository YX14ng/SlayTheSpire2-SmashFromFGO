using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Ejecución de la Guardiana — Ataque 3⚡: 26 de daño; contra Élites y Jefes: +12.
/// Mejora: 32 / +15.
/// </summary>
public sealed class GuardiansExecution() : ArtoriaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(26m, ValueProp.Move),
        new DynamicVar("Bonus", 12)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.Damage.BaseValue;
        if (Owner.Creature.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss)
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars["Bonus"].UpgradeValueBy(3m);
    }
}
