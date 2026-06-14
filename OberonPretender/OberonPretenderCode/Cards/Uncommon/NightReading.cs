using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Uncommon;

/// <summary>
/// Lectura Nocturna (夜读 / Night Reading) — DESIGN-OBERON §6.3. 2⚡ Poder: al inicio de tu turno robá
/// 1 carta adicional (la velocidad de mazo es el combustible del banco, P10 Morgan). Aplica
/// <see cref="NightReadingPower"/> (Counter: una 2ª copia roba +1). El up baja el coste a 1⚡.
/// </summary>
public sealed class NightReading() : OberonCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Draw", 1)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<NightReadingPower>(Owner.Creature, DynamicVars["Draw"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
