using System.Linq;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Basic;

/// <summary>
/// Firma S2: Mandato de la Reina (女王的敕命) — 5 de Bloqueo + Carga NP 10;
/// si algún enemigo tiene Maldición: +3 de Bloqueo. (S1 siembra → S2 cobra.)
/// Co-op: el mandato real carga al ejército — cada aliado gana !AllyCharge! de Carga NP.
/// </summary>
public sealed class QueensMandate() : MorganCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("NpCharge", 10),
        new DynamicVar("Bonus", 3),
        new DynamicVar("AllyCharge", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CursePower>()];

    // Glow del rediseño v2 (auditoría de completitud: era la única condicional sin brillo).
    protected override bool ShouldGlowGoldInternal =>
        Curses.CursedEnemies(Owner.Creature) > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        if (Curses.CursedEnemies(Owner.Creature) > 0)
        {
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Bonus"].BaseValue, ValueProp.Move, null);
        }
        await NpCharge.Gain(choiceContext, Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        // Co-op (mandato real que carga al ejército): cada aliado vivo gana un poco de Carga NP.
        // En 1 jugador, PlayerCreatures es solo el Owner -> el foreach queda vacío (fiel a 1 jugador).
        foreach (var ally in Owner.Creature.CombatState!.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
            await NpCharge.Gain(choiceContext, ally, DynamicVars["AllyCharge"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["NpCharge"].UpgradeValueBy(5m);
    }
}
