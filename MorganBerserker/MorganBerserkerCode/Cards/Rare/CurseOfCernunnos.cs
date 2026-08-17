using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MorganBerserker.MorganBerserkerCode.Powers;

namespace MorganBerserker.MorganBerserkerCode.Cards.Rare;

/// <summary>
/// Maldición de Cernunnos (科尔努诺斯的诅咒) — Poder 1⚡: cada Detonación tuya te da 10 de Carga NP
/// (mejora: 20). Re-efecto 2026-08-16: su «media detonación» se mudó a la Reina del Invierno
/// (ver <see cref="Powers.CurseOfCernunnosPower"/>).
/// </summary>
public sealed class CurseOfCernunnos() : MorganCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<CurseOfCernunnosPower>("Stacks", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromKeyword(MorganKeywords.Detonar), HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<CurseOfCernunnosPower>(choiceContext, Owner.Creature, DynamicVars["Stacks"].BaseValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Stacks"].UpgradeValueBy(10m);
    }
}
