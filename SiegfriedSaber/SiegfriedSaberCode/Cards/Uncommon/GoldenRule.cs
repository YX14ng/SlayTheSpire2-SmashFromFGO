using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using SiegfriedSaber.SiegfriedSaberCode.Cards;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Regla de Oro C- → Avaricia Dorada A (黄金律 / Golden Rule, DESIGN-SIEGFRIED §6) — el Rank-Up
/// como upgrade. Aplica <see cref="GoldenRulePower"/>: +50% a CADA ganancia de Carga NP del
/// combate (×NP, no ×daño global — gateado por generar el NP). El upgrade (Avaricia Dorada) no
/// sube el ratio (mantiene el techo §1.bis); añade +1 de Fuerza PERMANENTE al jugar (el "+ATK-up"
/// del rank-up, §6). Gateo del upgrade con IsUpgraded (NO el evento Upgraded).
/// </summary>
public sealed class GoldenRule() : SiegfriedCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GoldenRulePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GoldenRulePower>(Owner.Creature, 1m, Owner.Creature, this);
        if (IsUpgraded)
        {
            await PowerCmd.Apply<StrengthPower>(Owner.Creature, 1m, Owner.Creature, this);
        }
    }
}
