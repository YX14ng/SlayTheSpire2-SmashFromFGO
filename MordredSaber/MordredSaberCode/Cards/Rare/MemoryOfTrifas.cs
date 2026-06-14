using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Memoria de Trifas (特里法斯的记忆) — DESIGN-MORDRED §5.3. 2⚡ Poder: al inicio de cada turno curás 2 y
/// +5 de Carga NP (up: cura 3/+5). El epílogo: sustain conectado al hilo NP. Aplica
/// <see cref="MemoryOfTrifasPower"/> y fija HealPerTurn/NpPerTurn desde sus DynamicVars (patrón
/// WeightOfExpectations). El up sube SOLO la cura por activación.
/// </summary>
public sealed class MemoryOfTrifas() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Heal", 2), new DynamicVar("NpCharge", 5)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<MemoryOfTrifasPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.HealPerTurn = DynamicVars["Heal"].IntValue;
            power.NpPerTurn = DynamicVars["NpCharge"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Heal"].UpgradeValueBy(1m);
}
