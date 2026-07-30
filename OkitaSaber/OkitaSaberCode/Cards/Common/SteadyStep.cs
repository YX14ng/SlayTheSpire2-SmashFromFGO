using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Powers;

namespace OkitaSaber.OkitaSaberCode.Cards.Common;

/// <summary>
/// Paso Constante (稳步, KIT) — DESIGN-OKITA §5.3. 1⚡ Poder: aplica <see cref="SteadyStepPower"/>
/// (la primera *Ráfaga de cada turno reembolsa 1 *Aliento) (up: y +5★). Descuento de Aliento
/// capeado. Fija el RefundStarsValue del power desde el DynamicVar (0 base, 5 mejorada).
/// Movido a COMÚN (P2 2026-06-25): es el PEGAMENTO del arquetipo Ráfaga (sin él el arquetipo
/// colapsa por falta de Aliento). Como común aparece de forma fiable y deja de ser feast-or-famine.
/// </summary>
public sealed class SteadyStep() : OkitaCard(1, CardType.Power, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 0)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<AlientoPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<SteadyStepPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
            await power.Configure(choiceContext, DynamicVars["Stars"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Stars"].UpgradeValueBy(5m); // 0 -> +5★
}
