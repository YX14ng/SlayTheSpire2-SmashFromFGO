using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Cards.Basic;

/// <summary>
/// Palabra del Rey Hada (King Fae's Word) -- FIRMA basica de PRESTAMO (DESIGN-OBERON 6.1).
/// 1 Habilidad - PRESTAMO: +30 Carga NP; roba 1. Deuda 1. La 2a bateria del mazo inicial (doble
/// prestamo desde el turno 1) y la que ensena la economia-banco: recurso AHORA, Deuda al amanecer.
///
/// Prestamo chico (Deuda 1 = +10 NP al cobro): neto +20 si ahorras. <see cref="ILoanCard"/> ->
/// la dispara la pasiva del Rey del Cuento (1er prestamo del turno = endulzado), el Interes a Mi
/// Favor, y el limite de credito la apaga a Deuda >= 5 (glow off + no jugable: credito cortado).
/// </summary>
public sealed class KingFaesWord() : OberonCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self), ILoanCard
{
    private const int Charge = 30;
    public const int Debt = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Charge", Charge),
        new DynamicVar("Debt", Debt)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<DebtPower>()
    ];

    // Limite de credito (3.c): a Deuda >= 5 el prestamo deja de poder jugarse y su glow se apaga.
    protected override bool IsPlayable => !DebtPower.CreditCut(Owner.Creature);
    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["Charge"].IntValue, this);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
        await DebtPower.Add(Owner.Creature, DynamicVars["Debt"].IntValue, Owner.Creature, this);
    }

    // Up: +10 NP (la dosis sube; la Deuda 1 se mantiene -- el endulzante del Rey ya la calibra).
    protected override void OnUpgrade() => DynamicVars["Charge"].UpgradeValueBy(10m);
}
