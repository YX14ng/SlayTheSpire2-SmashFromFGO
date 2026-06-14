using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>El Tesoro Protege al Rey (宝物护王) — defensa-payoff de tribu. 1⚡ Hab: 5 de Bloqueo por cada
/// Arma del Tesoro jugada este turno (máx 4 Armas). Glow cuando jugaste un Arma. up: por-Arma 5→6.
/// Lee <see cref="ArmsPlayedPower"/>; INERTE hasta que el Arsenal alimente el contador (payoff puro de
/// tribu, sin cuerpo base, por diseño).</summary>
public sealed class TreasureGuardsTheKing() : GilgameshCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("PerArm", 5), new DynamicVar("MaxArms", 4)];

    protected override bool ShouldGlowGoldInternal => ArmsPlayedPower.PlayedThisTurn(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var arms = Math.Min(ArmsPlayedPower.PlayedThisTurn(Owner.Creature), DynamicVars["MaxArms"].IntValue);
        var block = arms * DynamicVars["PerArm"].IntValue;
        if (block > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, null);
        }
    }

    protected override void OnUpgrade() => DynamicVars["PerArm"].UpgradeValueBy(1m);
}
