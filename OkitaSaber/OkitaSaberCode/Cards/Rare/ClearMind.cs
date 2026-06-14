using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using OkitaSaber.OkitaSaberCode.Cards.Special;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Mente Despejada (明镜止水) — DESIGN-OKITA §5.4. 1⚡ Hab: robá 3; +20 Carga NP por cada *Tos en tu
/// mano tras robar (up: robá 4). Convierte el peor robo en combustible. Cuenta las Tos de la mano
/// DESPUÉS de robar (PlayerCombatState.Hand).
/// </summary>
public sealed class ClearMind() : OkitaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    private const int NpPerTos = 20;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        if (Owner.PlayerCombatState == null) return;
        var tosCount = Owner.PlayerCombatState.Hand.Cards.OfType<Tos>().Count();
        for (var i = 0; i < tosCount; i++)
        {
            await NpCharge.Gain(Owner.Creature, NpPerTos, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m); // robá 3 -> 4
}
