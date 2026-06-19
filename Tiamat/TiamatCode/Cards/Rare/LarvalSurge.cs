using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>Sobrecarga de la Larva — acelerador de Sobrecarga: cargás un gran bloque de NP de golpe
/// (+40) y, SI ese empujón cruza el umbral de manifestación (100), ganás Bendición de Sobrecarga,
/// que suma un turno a la próxima ventana Bestia (cada acumulación añade
/// <see cref="OverchargeBlessingPower.TierPerStack"/> al tier que consume el NP; la ventana dura
/// clamp(tier/100,1,3), así que +1 turno = +100 de tier). Premia banquear hacia las ventanas
/// largas en vez del daño plano.</summary>
public sealed class LarvalSurge() : TiamatCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    // +1 turno de ventana = +100 de tier. Derivado de TierPerStack para seguir correcto si se tunea.
    private static int BlessingStacks => 100 / OverchargeBlessingPower.TierPerStack;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 40)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // ¿El empujón CRUZA el umbral? (estaba por debajo de 100 y queda en 100+ tras cargar).
        var before = NpCharge.Current(Owner.Creature);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        var crossed = before < NpChargePower.ManifestThreshold
                      && NpCharge.Current(Owner.Creature) >= NpChargePower.ManifestThreshold;
        if (crossed)
        {
            await PowerCmd.Apply<OverchargeBlessingPower>(choiceContext, Owner.Creature, BlessingStacks, Owner.Creature, this);
        }
    }

    // Mejora: más Carga NP de golpe (más fácil cruzar el umbral y disparar la Bendición).
    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(15m);
}
