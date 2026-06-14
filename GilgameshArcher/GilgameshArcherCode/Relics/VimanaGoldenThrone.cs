using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Vimana, el Trono Dorado (维摩那·黄金王座) — reliquia POCO COMÚN (DESIGN-GILGAMESH §6). La primera vez en
/// cada combate que jugás 2+ Armas del Tesoro en un mismo turno: robás 2 (tempo de tribu, 1/combate).
/// Lee el contador <see cref="ArmsPlayedPower"/> YA existente en el mod (no requiere el módulo Arsenal;
/// degrada gracilmente a no-disparar hasta que existan las Armas). Patrón DasRheingold (AfterCardPlayed +
/// flag <see cref="_firedThisCombat"/>, reset en BeforeCombatStartLate).
/// </summary>
public sealed class VimanaGoldenThrone : GilgameshRelic
{
    private const int ArmsThreshold = 2;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private bool _firedThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public override Task BeforeCombatStartLate()
    {
        _firedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (_firedThisCombat || cardPlay.Card.Owner != Owner) return;
        if (ArmsPlayedPower.PlayedThisTurn(Owner.Creature) < ArmsThreshold) return;

        _firedThisCombat = true;
        Flash();
        await CardPileCmd.Draw(context, DynamicVars.Cards.IntValue, Owner);
    }
}
