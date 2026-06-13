using System.Collections.Generic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using SiegfriedSaber.SiegfriedSaberCode.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Cards.Special;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Por Convicción Propia (§7) — "su sueño hecho carta de decisión". Exhaust. ELEGÍ una: +50 Carga NP o +3
/// Sangre de Dragón. Mejorada: ganás AMBAS (sin elección). Patrón de elección WineFox: arma option-cards Token
/// y las muestra con FromChooseACardScreen; la elegida aplica su efecto vía IConvictionOption. (La 3ª opción
/// del diseño —próximo Ataque Cazadragones ×2— queda diferida hasta tener el power con tope +12.)
/// </summary>
public sealed class ByMyConviction() : SiegfriedCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<DragonScalesPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (IsUpgraded)
        {
            // "Elegí DOS" de dos = ambas, sin pantalla de elección.
            await NpCharge.Gain(Owner.Creature, ConvictionCharge.NpGain, this);
            await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, ConvictionScales.ScalesGain, Owner.Creature, this);
            return;
        }

        var combatState = Owner.Creature.CombatState;
        if (combatState == null) return;

        var options = new List<CardModel>
        {
            combatState.CreateCard(ModelDb.Card<ConvictionCharge>(), Owner),
            combatState.CreateCard(ModelDb.Card<ConvictionScales>(), Owner),
        };
        var chosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, options, Owner, false);
        if (chosen is IConvictionOption option) await option.ApplyConviction();
    }
}
