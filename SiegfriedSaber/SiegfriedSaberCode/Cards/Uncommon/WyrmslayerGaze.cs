using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Ojo del Mataguivernos (屠龙者之眼 / Wyrmslayer's Gaze) — el SINK de Estrellas de Siegfried
/// (DESIGN-REVIEW-2: las ★ no tenían dónde gastarse; PiercingGlare las LEE, ésta las GASTA). 1⚡ At: 6 de
/// daño; consumí hasta <see cref="StarsSpent"/> ★ e inflige 1 de daño más por cada 10★ gastadas (máx
/// <see cref="MaxBonus"/>). El golpe certero que descarga la luz de estrellas acumulada en un solo tajo:
/// convierte la banca de ★ en burst, igual que las cartas con "Crítico" del ecosistema. Como Siegfried
/// NO banca (sin IBanksCritStars), las ★ igual se auto-procesan a 100 → este sink compite con ese
/// auto-payoff, dándote la ELECCIÓN de gastarlas antes en daño dirigido. No genera ★ (no se
/// auto-alimenta). Glow cuando hay ★ que gastar. El up sube el daño base (el ratio ★→daño queda fijo).
/// </summary>
public sealed class WyrmslayerGaze() : SiegfriedCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private const int StarsSpent = 50;  // ★ máximas que consume
    private const int StarsPerDamage = 10; // 10★ → +1 daño
    private const int MaxBonus = 5;     // tope del bonus (techo §1.bis)

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(6m, ValueProp.Move), new DynamicVar("Spend", StarsSpent), new DynamicVar("MaxBonus", MaxBonus)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool ShouldGlowGoldInternal => CritStars.Of(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Consumí hasta Spend ★ (lo que haya); el daño extra escala con lo gastado, con tope MaxBonus.
        var spent = System.Math.Min(CritStars.Of(Owner.Creature), DynamicVars["Spend"].IntValue);
        if (spent > 0) await CritStars.Gain(choiceContext, Owner.Creature, -spent, this);
        var bonus = System.Math.Min(spent / StarsPerDamage, DynamicVars["MaxBonus"].IntValue);

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
