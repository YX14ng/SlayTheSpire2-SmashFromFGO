using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Uncommon;

/// <summary>Disparo Anticipado (先制射击) — secuenciado. 1⚡ At: 9 de daño; si es la 1ª carta del turno,
/// +20 Estrellas (up 12 / +30). Glow cuando es la primera carta. Usa <see cref="CardsThisTurnPower"/>
/// (PlayedBefore==0) — el motor expone "primera carta del turno" que el engine no da.</summary>
public sealed class AnticipatedShot() : GilgameshCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DamageVar(9m, ValueProp.Move), new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritStarsPower>()];

    // Fail-CLOSED (audit 2026-07-05): sin el contador instalado NO sabemos cuantas cartas jugaste —
    // PlayedBefore==0 por ausencia contaba SIEMPRE como primera. Exige el power presente.
    private bool IsFirstCard => Owner.Creature.GetPower<CardsThisTurnPower>() is { Played: 0 };

    protected override bool ShouldGlowGoldInternal => IsFirstCard;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        // Capturado ANTES de instalar/pegar: instalar recien aca no puede otorgar el bonus (fail-closed).
        var first = IsFirstCard;
        await CardsThisTurnPower.EnsureInstalled(Owner.Creature);

        await AttackTarget(choiceContext, cardPlay.Target, DynamicVars.Damage.BaseValue);

        if (first)
        {
            await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Stars"].UpgradeValueBy(10m);
    }
}
