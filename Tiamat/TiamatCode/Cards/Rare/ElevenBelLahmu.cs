using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace TiamatBeast.TiamatCode.Cards.Rare;

/// <summary>Once Bel Laḫmu — repoblás el enjambre hasta el tope (6) y ganás 1 Crianza por cada hueco
/// que rellenaste (por cada faltante): cuanto más vacío estabas, más Crianza acuñás. La explosión de
/// población que prepara la mordida/devorada. Exhaust.</summary>
public sealed class ElevenBelLahmu() : TiamatCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Lahmu", 6),
        new DynamicVar("NurturePerSpawn", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<LahmuSwarmPower>(), HoverTipFactory.FromPower<LahmuNurturePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Parí hasta el tope; el helper devuelve cuántos cupieron de verdad (= huecos rellenados).
        var spawned = await Lahmu.Spawn(Owner.Creature, DynamicVars["Lahmu"].IntValue, this);
        if (spawned > 0)
        {
            await Lahmu.Feed(Owner.Creature, spawned * DynamicVars["NurturePerSpawn"].IntValue, this);
        }
    }

    // Mejora: +1 Crianza por hueco rellenado (escala el motor de población más rápido).
    protected override void OnUpgrade() => DynamicVars["NurturePerSpawn"].UpgradeValueBy(1m);
}
