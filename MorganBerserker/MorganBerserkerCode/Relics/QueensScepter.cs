using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Rhongomyniad, el Cetro de la Reina (止境之枪·王笏) — starter relic, rediseño v2
/// (lección 焰刑地狱: la starter convierte eventos universales en recursos del kit):
/// (1) MANTIENE: at combat start enter Fairy Queen form (and kick off FormVisuals'
/// background preload) — without it Morgan fought FORMLESS until her first switch.
/// (2) MANTIENE: the first time you change form each combat: +1 Energy, draw 1
/// and NP +10 (makes the first switch tempo-positive).
/// (3) AGREGA: every time Morgan loses HP (any source — enemy attacks, self-damage):
/// +10 Critical Stars, capped at 3 events per turn (parche P2, máx +30/turno —
/// la calibración "tanquear 2-3 golpes"; mismo patrón _triggersThisTurn que
/// MadnessEnhancementPower). Sangrar → estrellas: el espejo Berserker del
/// "perder HP → +NP" de Jeanne. Parche P4: el tick de FaeBloodPact NO cuenta.
/// </summary>
public sealed class QueensScepter : MorganRelic, IFormChangeListener
{
    public const int NpOnFirstSwitch = 10;
    public const int StarsPerHpLoss = 10;
    public const int StarTriggersPerTurn = 3;

    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    private bool _usedThisCombat;
    private int _starTriggersThisTurn;

    public override async Task BeforeCombatStartLate()
    {
        _usedThisCombat = false;
        _starTriggersThisTurn = 0;
        // Forma inicial: Reina. source == null -> no cuenta como "cambio de forma".
        await FormSwitch.Enter<Powers.Forms.FairyQueenFormPower>(null, Owner.Creature, null);
    }

    public override Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // Tope P2 por RONDA: se resetea al inicio del turno del jugador y cuenta
        // tanto el autodaño propio como los golpes tanqueados en el turno enemigo.
        if (side == CombatSide.Player) _starTriggersThisTurn = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature || result.UnblockedDamage <= 0) return;
        if (Powers.FaeBloodPactPower.TickInProgress) return; // P4: el tick del Pacto no genera estrellas.
        if (_starTriggersThisTurn >= StarTriggersPerTurn) return;

        _starTriggersThisTurn++;
        Flash();
        await CritStars.Gain(Owner.Creature, StarsPerHpLoss, null);
    }

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (_usedThisCombat) return;
        _usedThisCombat = true;
        Flash();
        await PlayerCmd.GainEnergy(1, Owner);
        await NpCharge.Gain(Owner.Creature, NpOnFirstSwitch, null);
        if (choiceContext != null)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }
}
