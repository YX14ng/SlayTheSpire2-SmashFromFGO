using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Sangre de Dragón (龙之血) — DESIGN-MORDRED §5.2. 1⚡ Poder: al inicio de cada turno +5 de Carga NP
/// y 3 de Bloqueo (up: +5 NP / +5 Bloqueo). El trait Dragon (vitalidad de homúnculo dracónico): engorda
/// los hilos existentes (medidor + tanqueo). Aplica <see cref="DragonsBloodPower"/>; los valores por
/// activación son campos settables que la carta fija desde sus DynamicVars. Patrón GoldenRule/MemoryOfTrifas.
/// </summary>
public sealed class DragonsBlood() : MordredCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 5), new DynamicVar("Block", 3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<DragonsBloodPower>(Owner.Creature, 1m, Owner.Creature, this);
        if (power != null)
        {
            power.NpPerTurn = DynamicVars["NpCharge"].IntValue;
            power.BlockPerTurn = DynamicVars["Block"].IntValue;
        }
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}
