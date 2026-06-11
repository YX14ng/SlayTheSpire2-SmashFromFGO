using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Uncommon;

/// <summary>
/// Carisma de la Esperanza B (skill real S1 de Castoria) — 2 de Fuerza; Carga NP +30.
/// Co-op: cada aliado gana 1 de Fuerza y Carga NP +10. Exhaust.
/// </summary>
public sealed class CharismaOfHope() : ArtoriaCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<StrengthPower>("Strength", 2m),
        new DynamicVar("NpCharge", 30),
        new DynamicVar("AllyStrength", 1),
        new DynamicVar("AllyNpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);

        // Co-op: el carisma alcanza a todo el party.
        foreach (var player in Owner.RunState.Players)
        {
            if (player == Owner || player.Creature.IsDead) continue;
            await PowerCmd.Apply<StrengthPower>(player.Creature, DynamicVars["AllyStrength"].BaseValue, Owner.Creature, this);
            await NpCharge.Gain(player.Creature, DynamicVars["AllyNpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
