using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Carisma A+ (卡里斯玛 A+) — la skill real S1, con Exhaust como cooldown (regla Artoria).
/// 2⚡ Hab Exhaust: 2 de Fuerza + 20 Carga NP (up 3 / +30). El bono de aliados en co-op queda diferido
/// (fiel en un jugador).</summary>
public sealed class KitCharisma() : GilgameshCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<StrengthPower>("Strength", 2m), new DynamicVar("Np", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<StrengthPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(Owner.Creature, DynamicVars["Strength"].BaseValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
        // Co-op (DESIGN-GILGAMESH §6 «buffer secundario»): Carisma le da 1 de Fuerza a CADA aliado.
        // En 1 jugador, PlayerCreatures es solo el Owner -> el foreach queda vacío (fiel a 1 jugador).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
            await PowerCmd.Apply<StrengthPower>(ally, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1m);
        DynamicVars["Np"].UpgradeValueBy(10m);
    }
}
