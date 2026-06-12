using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MashShielder.MashShielderCode.Cards.Common;

/// <summary>
/// Carga Frontal — rediseño v2: 2E Ataque, 12 daño (up +4). Si conservaste Bloqueo
/// del turno anterior (Baluarte): +6 daño y +10 de Carga NP (up: +20). EL payoff
/// explícito de Baluarte en común, calibrado a la retención de 10 de la starter:
/// casi siempre encendido si jugás el plan, nunca gratis.
/// </summary>
public sealed class FrontalCharge() : MashShielderCard(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(12m, ValueProp.Move),
        new DynamicVar("BonusDamage", 6),
        new DynamicVar("NpCharge", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<BulwarkPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    /// <summary>
    /// «Conservaste Bloqueo del turno anterior»: el Bloqueo actual excede lo ganado
    /// este turno (historial de combate), es decir, todavía sostenés muro retenido.
    /// Exacto: block = retenido + ganadoTurno - perdidoTurno → block &gt; ganadoTurno
    /// ⇔ retenido &gt; perdidoTurno.
    /// </summary>
    private bool KeptBlockFromLastTurn
    {
        get
        {
            var state = CombatState;
            if (state == null || state.RoundNumber <= 1) return false;

            var creature = Owner.Creature;
            var gainedThisTurn = CombatManager.Instance.History.Entries
                .OfType<BlockGainedEntry>()
                .Where(e => e.Receiver == creature && e.HappenedThisTurn(state))
                .Sum(e => e.Amount);
            return creature.Block > gainedThisTurn;
        }
    }

    protected override bool ShouldGlowGoldInternal => KeptBlockFromLastTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var kept = KeptBlockFromLastTurn;
        var bonus = kept ? DynamicVars["BonusDamage"].IntValue : 0;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + bonus).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt")
            .Execute(choiceContext);

        if (kept)
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
