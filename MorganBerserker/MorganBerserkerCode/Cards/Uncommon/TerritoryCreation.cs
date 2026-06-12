using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Creación de Territorio (阵地作成) — Poder: al final de tu turno: 4 de Bloqueo
/// + 5 de Carga NP. Rediseño v2: el territorio canaliza maná (era isla de Bloqueo
/// puro, ahora hilo Bloqueo→NP). (up +2 Bloqueo)
/// </summary>
public sealed class TerritoryCreation() : MorganCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<TerritoryCreationPower>("Stacks", 4m),
        new DynamicVar("NpCharge", TerritoryCreationPower.NpPerTurn)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TerritoryCreationPower>(Owner.Creature, DynamicVars["Stacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(2m);
    }
}
