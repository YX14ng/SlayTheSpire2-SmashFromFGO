using MashShielder.MashShielderCode.Powers;
using MashShielder.MashShielderCode.Powers.Forms;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Uncommon;

/// <summary>Simulacro de Cambio — NUEVA del rediseño v2, reemplaza a TrainingSimulation
/// (hereda su slot de arte: el simulador de entrenamiento).
/// 0E Hab: cambia a la otra forma (Shielder↔Ortinax; en Paladín: +10 NP en su lugar).
/// Up (parche P2 del juez): roba 1 — el robo NO va en base para que el ping-pong con
/// HomunculusHeart no sea un motor de robo a 0E. EL pan-y-manteca de formas que faltaba.</summary>
public sealed class FormDrill() : MashShielderCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("NpCharge", 10),
        new CardsVar(0)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShielderFormPower>(),
        HoverTipFactory.FromPower<OrtinaxFormPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Creature.HasPower<PaladinFormPower>())
        {
            // Paladín es permanente: no hay vuelta atrás — el simulacro carga el medidor.
            await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        }
        else if (Owner.Creature.HasPower<OrtinaxFormPower>())
        {
            await Forms.Enter<ShielderFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await Forms.Enter<OrtinaxFormPower>(choiceContext, Owner.Creature, this);
        }

        if (DynamicVars.Cards.BaseValue > 0)
        {
            await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
