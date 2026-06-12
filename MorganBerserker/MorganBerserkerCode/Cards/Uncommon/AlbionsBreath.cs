using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Aliento de Albion (阿尔比恩的吐息) — rediseño v2: 9 de daño ×3 al mismo objetivo.
/// Si tenés CRÍTICO LISTO: lo consume y LOS TRES golpes critican (×2) — la semántica
/// "1 stack por CARTA, todos sus golpes" ya vive en FGOCore.CritReadyPower (parche P8:
/// sin TryConsume manual ni riesgo de ×4), así que acá solo se enseña/señaliza:
/// glow dorado con Crítico Listo en cola. (up: daño 9→11)
/// </summary>
public sealed class AlbionsBreath() : MorganCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.HasPower<CritReadyPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).WithHitCount(3).FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_heavy_blunt")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
