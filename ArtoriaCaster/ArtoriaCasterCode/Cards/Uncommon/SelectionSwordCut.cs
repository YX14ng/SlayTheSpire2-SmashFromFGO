using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Corte de la Espada de Selección — 16 de daño. Crítico 4★: 30 (el pago grande
/// de la economía de estrellas en poco comunes).
/// </summary>
public sealed class SelectionSwordCut() : ArtoriaCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public const int CritCost = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move),
        new DynamicVar("Crit", 30),
        new DynamicVar("CritCost", CritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = DynamicVars.Damage.BaseValue;
        if (Stars.CanCrit(Owner.Creature, CritCost))
        {
            await Stars.ConsumeForCrit(Owner.Creature, CritCost, this);
            damage = DynamicVars["Crit"].BaseValue + Stars.CritBonus(Owner.Creature);
        }
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Crit"].UpgradeValueBy(8m);
    }
}
