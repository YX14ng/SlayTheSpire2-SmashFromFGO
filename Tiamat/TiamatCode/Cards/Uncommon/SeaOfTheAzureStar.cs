using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Mar de la Estrella Azur — el ÚNICO soporte de mar del roster (REDESIGN-TIAMAT §43). El Mar de
/// Vida sana: curás !Heal! HP y obtenés !NpCharge! de Carga NP. En co-op, el mar baña a TODA la
/// party: cada jugador-aliado también sana y carga (los enemigos NO). Sustain + aceleración de
/// medidor que NO empuja daño plano.
/// </summary>
public sealed class SeaOfTheAzureStar() : TiamatCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(8m),
        new DynamicVar("NpCharge", 12)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var heal = DynamicVars.Heal.BaseValue;
        var np = DynamicVars["NpCharge"].IntValue;
        // El mar baña a toda la party: en 1 jugador es solo Tiamat; en co-op incluye a los aliados.
        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.ToList())
        {
            if (ally.IsDead) continue;
            await CreatureCmd.Heal(ally, heal);
            await NpCharge.Gain(choiceContext, ally, np, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Heal.UpgradeValueBy(3m);
        DynamicVars["NpCharge"].UpgradeValueBy(4m);
    }
}
