using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Ambición del Trono (王座野望) — DESIGN-MORDRED §5.3. 2⚡ Poder: al inicio de cada turno +10 de Carga
/// NP (up +15). Per-turn NP, slot Bendición de Avalon. Aplica <see cref="AmbitionForThronePower"/> y fija
/// su NpPerTurn desde el DynamicVar (patrón WeightOfExpectations). Sube SOLO el valor por activación.
/// </summary>
public sealed class AmbitionForTheThrone() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<AmbitionForThronePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null) power.NpPerTurn = DynamicVars["NpCharge"].IntValue;
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(5m);
}
