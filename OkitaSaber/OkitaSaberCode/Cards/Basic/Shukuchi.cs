using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OkitaSaber.OkitaSaberCode.Cards;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Basic;

/// <summary>
/// FIRMA «Shukuchi» (缩地) — la firma básica que enseña las dos identidades de Okita desde el
/// combate 1 (DESIGN-OKITA §5.1): es una *RÁFAGA (doble coste ⚡ + 1 *Aliento) y ATACAR = GENERAR
/// (★). 5 daño ×2; +10★ (up: 7×2; +20★). El paso que acorta la tierra.
/// </summary>
public sealed class Shukuchi() : OkitaCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), IRafagaCard
{
    public int RafagaCost => 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5m, ValueProp.Move),
        new DynamicVar("Hits", 2),
        new DynamicVar("Stars", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    protected override bool IsPlayable => Rafaga.IsPlayable(Owner.Creature, RafagaCost);

    // Glow dorado cuando el Aliento alcanza (la Ráfaga es jugable).
    protected override bool ShouldGlowGoldInternal => Rafaga.ShouldGlow(Owner.Creature, RafagaCost);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Segundo coste de la Ráfaga (Aliento / override de forma / HP).
        await Rafaga.Pay(choiceContext, Owner.Creature, RafagaCost, this);

        var hits = DynamicVars["Hits"].IntValue;
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
        }
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m); // 5 -> 7
        DynamicVars["Stars"].UpgradeValueBy(10m); // +10 -> +20
    }
}
