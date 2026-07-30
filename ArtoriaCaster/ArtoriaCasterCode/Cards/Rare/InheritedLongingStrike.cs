using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Golpe del Anhelo Heredado — Ataque 2⚡: 14 de daño. Crítico ESCALABLE 3★: 24, +4 por cada ★
/// extra hasta 5★ (techo 32). Mejora: 18 base / 28 al mínimo (techo 36).
/// Rediseño P2 2026-06-25: era un crítico binario con umbral 4★ (feast-or-famine: 0 valor-crit
/// por debajo de 4★, salto a 32 exacto). Ahora arranca a 3★ y escala con las ★ extra — misma
/// cima, mucha menos varianza (modelo <see cref="ArtoriaCard.ResolveCritDamageScaling"/>).
/// </summary>
public sealed class InheritedLongingStrike() : ArtoriaCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public const int CritCost = 3;
    public const int MaxCritCost = 5;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("Crit", 24),
        new DynamicVar("PerStar", 4),
        new DynamicVar("CritCost", CritCost),
        new DynamicVar("MaxCritCost", MaxCritCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CriticalStarsPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = await ResolveCritDamageScaling(CritCost, MaxCritCost);
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Crit"].UpgradeValueBy(4m);
    }
}
