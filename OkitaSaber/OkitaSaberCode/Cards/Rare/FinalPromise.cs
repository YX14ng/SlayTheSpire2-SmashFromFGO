using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace OkitaSaber.OkitaSaberCode.Cards.Rare;

/// <summary>
/// Última Promesa (最后的约定) — DESIGN-OKITA §5.4. 1⚡ Hab, Exhaust: +1 Alzarse (<see cref="GutsPower"/>);
/// +20 Carga NP (up: +30 NP). "Luchar hasta el final" — el capstone del vínculo.
/// </summary>
public sealed class FinalPromise() : OkitaCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<GutsPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GutsPower>(Owner.Creature, 1m, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m); // +20 -> +30
}
