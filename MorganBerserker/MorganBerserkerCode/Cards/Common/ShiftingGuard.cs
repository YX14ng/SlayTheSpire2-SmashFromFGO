using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>
/// Guardia Cambiante (变换守势) — RE-POOL V2 [NUEVA] (§5.1): Habilidad 1⚡: 5 de Bloqueo; si
/// cambiaste de forma este turno: 10 (mejora 7/13). La defensa DE la danza — el arquetipo A
/// defendía con cartas ajenas. Flag = bit 9 del estado de turno (cetro/Ancient). Glow con flag.
/// </summary>
public sealed class ShiftingGuard() : MorganCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("BigBlock", 10)
    ];

    private bool ShiftedThisTurn => FgoCombatState.GetTurn(Owner.Creature, 9) != 0;

    protected override bool ShouldGlowGoldInternal => ShiftedThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = ShiftedThisTurn ? DynamicVars["BigBlock"].BaseValue : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["BigBlock"].UpgradeValueBy(3m);
    }
}
