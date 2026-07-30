using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Corte del Genio (天才之斩) — DESIGN-OKITA §5.4. 3⚡ At: 30 daño; si tenés CRÍTICO LISTO: este ataque
/// IGNORA Bloqueo (daño directo Unblockable, patrón <see cref="Special.MumyouUnleashed"/>) (up: 38).
/// Slot boss-killer condicional. Glow cuando hay Crítico Listo (el ×2 de CritReadyPower también
/// aplica al daño Unblockable).
/// </summary>
public sealed class GeniusCut() : OkitaCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(30m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    private bool WillCrit => Criticals.WillCrit(Owner.Creature, this);

    protected override bool ShouldGlowGoldInternal => WillCrit;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        if (Criticals.IsCritical(cardPlay))
        {
            // IGNORA Bloqueo: daño directo Unblockable (el ×2 de CritReadyPower lo dobla igual).
            VfxCmd.PlayOnCreatureCenter(cardPlay.Target, "vfx/vfx_dramatic_stab");
            await CreatureCmdCompatibility.Damage(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue,
                ValueProp.Move | ValueProp.Unblockable, Owner.Creature, this, cardPlay);
        }
        else
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_dramatic_stab")
                .Execute(choiceContext);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(8m); // 30 -> 38
}
