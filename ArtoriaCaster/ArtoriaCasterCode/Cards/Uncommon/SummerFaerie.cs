using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Hada del Verano B (skill real S2 del Berserker de verano) — Carga NP +15; 2 de
/// Bendición de Sobrecarga. Co-op: un aliado aleatorio gana Carga NP +10. Exhaust.
/// </summary>
public sealed class SummerFaerie() : ArtoriaCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 15),
        new PowerVar<OverchargeBlessingPower>("Blessing", 2m),
        new DynamicVar("AllyNpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<OverchargeBlessingPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner.Creature, DynamicVars["Blessing"].BaseValue, Owner.Creature, this);

        // Co-op: un aliado aleatorio recibe el regalo del hada.
        var allies = Owner.RunState.Players.Where(p => p != Owner && !p.Creature.IsDead).ToList();
        if (allies.Count > 0)
        {
            var ally = allies[Owner.RunState.Rng.CombatCardGeneration.NextInt(allies.Count)];
            await NpCharge.Gain(ally.Creature, DynamicVars["AllyNpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
