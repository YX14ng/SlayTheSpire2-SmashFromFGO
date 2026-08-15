using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MorganBerserker.MorganBerserkerCode.Powers.Forms;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Coronación del Confín (止境的加冕) — Ancient (reemplazo de Orobas del cetro). Re-efecto
/// 2026-08-15 (REDESIGN-MORGAN-V2 §6, parches J2-1/J3-4 — el hallazgo estructural del panel):
/// REINSTALA TODO el motor del cetro que su reemplazo físico amputaba — arranque en Reina Hada,
/// Metamorfosis gratis del turno 1, re-armado a 100 NP (M3, lo maneja MainFile leyendo esta
/// reliquia) y «perder HP → 3 Maldición a un enemigo aleatorio, cap 3/turno»
/// (<see cref="ScepterSeed"/>) — y CONSERVA su premio propio: +1 Energía por cambio de forma
/// (máx. 1/turno). El «+5 al cap de Maldición» propuesto quedó DESCARTADO (el cap es const en
/// FGOCore, sin hook — J1-5/J3-3).
/// </summary>
public sealed class WorldsEndCoronation : MorganRelic, IFormChangeListener
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeCombatStartLate()
    {
        await FormSwitch.Enter<FairyQueenFormPower>(null, Owner.Creature, null);
    }

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner || FgoCombatState.GetCombat(Owner.Creature, 1) != 0) return;
        await FgoCombatState.SetCombat(choiceContext, Owner.Creature, 1, 1);
        await FGOCore.FGOCoreCode.Combat.ManifestCards
            .ManifestToHand<Cards.Special.QueensMetamorphosis>(Owner.Creature, 1.0f);
    }

    public override Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource) =>
        ScepterSeed.OnHpLoss(this, choiceContext, target, result, cardSource);

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        // RE-POOL V2: bit 9 = «cambiaste de forma este turno» (ver QueensScepter.OnFormChanged).
        if (FgoCombatState.GetTurn(Owner.Creature, 9) == 0)
        {
            await FgoCombatState.SetTurn(
                choiceContext ?? new BlockingPlayerChoiceContext(), Owner.Creature, 9, 1);
        }
        if (FgoCombatState.GetTurn(Owner.Creature, 4) != 0) return;
        await FgoCombatState.SetTurn(
            choiceContext ?? new BlockingPlayerChoiceContext(), Owner.Creature, 4, 1);
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
    }
}
