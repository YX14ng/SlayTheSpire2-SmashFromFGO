using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Garra de Melusine (梅柳齐娜之爪) — re-pool V2 (rider de P1, parche J2-11): 8 de daño + 10 de
/// Carga NP; si tenés un Arma del Caballero en la mano: +5 (mejora 11/+6). Melusine, la más leal
/// de los Fairy Knights, pega más fuerte cuando la corte está convocada. Glow con Arm en mano.
/// </summary>
public sealed class MelusinesTalon() : MorganCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(8m, ValueProp.Move),
        new DynamicVar("NpCharge", 10),
        new DynamicVar("Bonus", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    private bool ArmInHand =>
        Owner.Creature.Player?.PlayerCombatState?.AllPiles
            .FirstOrDefault(p => p.Type == PileType.Hand)?.Cards
            .Any(c => c is Special.KnightsArm) == true;

    protected override bool ShouldGlowGoldInternal => ArmInHand;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = DynamicVars.Damage.BaseValue;
        if (ArmInHand)
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Bonus"].UpgradeValueBy(1m);
    }
}
