using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Duelo Bajo la Nieve (雪中之决) — DESIGN-OKITA §5.4. 2⚡ At: 14 daño; contra Élites y Jefes: +10
/// (up: 18 / +12). La nieve de Ikedaya; boss-killer. Glow en Élite/Jefe (lectura de RoomType,
/// patrón GuardiansExecution).
/// </summary>
public sealed class DuelInTheSnow() : OkitaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(14m, ValueProp.Move), new DynamicVar("Bonus", 10)];

    private bool VsEliteOrBoss =>
        Owner.Creature.CombatState?.Encounter?.RoomType is RoomType.Elite or RoomType.Boss;

    protected override bool ShouldGlowGoldInternal => VsEliteOrBoss;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = DynamicVars.Damage.BaseValue + (VsEliteOrBoss ? DynamicVars["Bonus"].BaseValue : 0m);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_dramatic_stab")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);    // 14 -> 18
        DynamicVars["Bonus"].UpgradeValueBy(2m);  // +10 -> +12
    }
}
