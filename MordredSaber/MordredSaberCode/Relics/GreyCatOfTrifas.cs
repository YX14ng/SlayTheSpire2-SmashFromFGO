using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace MordredSaber.MordredSaberCode.Relics;

/// <summary>
/// Gato Gris de Trifas (特里法斯的灰猫) — reliquia RARA (DESIGN-MORDRED §6): al final de tu turno, si
/// NO jugaste ningún Ataque, +<see cref="Stars"/> Estrellas de Crítico y curás <see cref="HealAmount"/>
/// (el turno de tregua — la paz fugaz de Trifas). Premia los turnos puramente defensivos/de carga, sin
/// loop con el motor (la cura es delta positivo, no proca el ★-por-perder-Vida de la starter). Patrón
/// FamiliarOwl de Artoria (flag _attackPlayedThisTurn vía AfterCardPlayed, dispara en AfterTurnEnd).
/// </summary>
public sealed class GreyCatOfTrifas : MordredRelic
{
    private const int Stars = 10;
    private const int HealAmount = 2;

    public override RelicRarity Rarity => RelicRarity.Rare;

    private bool _attackPlayedThisTurn;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    public override Task BeforeCombatStartLate()
    {
        _attackPlayedThisTurn = false;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _attackPlayedThisTurn = true;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player) return;
        var played = _attackPlayedThisTurn;
        _attackPlayedThisTurn = false;
        if (played) return;
        Flash();
        await CritStars.Gain(Owner.Creature, Stars, null);
        await CreatureCmd.Heal(Owner.Creature, HealAmount);
    }
}
