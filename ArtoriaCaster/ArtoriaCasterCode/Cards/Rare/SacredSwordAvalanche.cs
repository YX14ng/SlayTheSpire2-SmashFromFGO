using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Avalancha de Espadas Sagradas — Ataque 2⚡: 7 de daño ×3 al MISMO objetivo.
/// Crítico 3★ (se consume UNA vez): cada golpe +4. Mejora: 8×3 / +5.
/// El bonus plano de crítico (ICritDamageBoost) se aplica una sola vez, al primer
/// golpe, para mantener la paridad con las cartas de un golpe.
/// </summary>
public sealed class SacredSwordAvalanche() : ArtoriaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int CritCost = 3;

    private const int Hits = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar("Crit", 4),
        new DynamicVar("CritCost", CritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var perHit = DynamicVars.Damage.BaseValue;
        var firstHitBonus = 0m;
        if (Stars.CanCrit(Owner.Creature, CritCost))
        {
            await Stars.ConsumeForCrit(Owner.Creature, CritCost, this);
            perHit += DynamicVars["Crit"].BaseValue;
            firstHitBonus = Stars.CritBonus(Owner.Creature);
        }

        for (var i = 0; i < Hits; i++)
        {
            if (cardPlay.Target.IsDead) break;
            await DamageCmd.Attack(perHit + (i == 0 ? firstHitBonus : 0m)).FromCard(this)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["Crit"].UpgradeValueBy(1m);
    }
}
