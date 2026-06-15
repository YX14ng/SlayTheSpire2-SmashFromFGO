using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using FGOCore.FGOCoreCode.Cleanse;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Rare;

/// <summary>
/// Retirada Estratégica (战略撤退, §9) — UNA de las dos únicas cartas de cleanse del pool (P8). Bloqueo
/// Baluarte (persiste al próximo turno) + limpia TODOS tus Debuffs. Exhaust = cooldown (un uso/combate): el
/// sobrecosto estructural que balancea el cleanse, fiel a "Siegfried es vulnerable a debuffs por diseño".
/// </summary>
public sealed class StrategicWithdrawal() : SiegfriedCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(18m, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await BlockRetention.GainBulwarkBlock(this, Owner.Creature, DynamicVars.Block.BaseValue);
        await Cleanse.RemoveDebuffs(Owner.Creature);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(6m);
}
