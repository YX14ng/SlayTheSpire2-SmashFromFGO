using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using FGOCore.FGOCoreCode.Np;

namespace FGOCore.FGOCoreCode.Memes;

/// <summary>
/// El Elemento Imaginario (虚数魔术) — al jugarla: +50 de Carga NP y, además, la Carga NP que
/// ganás el resto de ESTE turno se incrementa +30% (un <see cref="FormalCraftPower"/> corto).
/// Burst de carga + ventana de generación aumentada para encadenar otros generadores el mismo
/// turno. Exhaust + Poco común lo pagan.
/// </summary>
public sealed class ImaginaryElement() : MemeCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("NpCharge", 50)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<FormalCraftPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // La carga base de ESTA carta NO se auto-amplifica: primero el +50 plano, después el
        // buff, así el +30% rige solo para los generadores SIGUIENTES del turno.
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        await PowerCmd.Apply<FormalCraftPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["NpCharge"].UpgradeValueBy(20m);
    }
}
