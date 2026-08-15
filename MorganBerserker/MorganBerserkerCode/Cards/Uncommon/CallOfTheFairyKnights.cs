using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Llamado de los Caballeros Hada (妖精骑士召集) — 2 de Maldición a TODOS (Barghest),
/// 1 de Débil a TODOS (Baobhan Sith), 6 de Bloqueo (Melusine). Exhaust.
/// Rediseño v2: además añade 1 Arma del Caballero a tu mano (los caballeros traen
/// sus espadas — 2º generador de la tribu, en poco común). (up +1/+1/+3)
/// </summary>
public sealed class CallOfTheFairyKnights() : MorganCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Curse", 3),
        new PowerVar<WeakPower>("Weak", 1m),
        new BlockVar(6m, ValueProp.Move),
        new CardsVar(1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CursePower>(), HoverTipFactory.FromPower<WeakPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await Curses.Apply(choiceContext, enemy, DynamicVars["Curse"].IntValue, Owner.Creature, this);
            }
        }
        foreach (var enemy in Owner.Creature.CombatState!.GetOpponentsOf(Owner.Creature))
        {
            if (!enemy.IsDead)
            {
                await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
            }
        }
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await Special.KnightsArm.AddToHand(Owner.Creature, DynamicVars.Cards.IntValue);
    }

    // RE-POOL V2: la mejora trae más caballeros (+1 Arm), no más números.
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1m);
    }
}
