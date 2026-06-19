using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Núcleo de la Diosa — el ancla anti-control de Tiamat (REDESIGN-TIAMAT §44). Obtenés
/// Resistencia a Debuffs (Artefacto: anula los próximos !Artifact!) y un Núcleo que bombea
/// !NpPerTurn! de Carga NP al inicio de tus turnos. La diosa-larva endurece su núcleo y deja
/// que el medidor se llene sola.
/// </summary>
public sealed class CoreOfTheGoddess() : TiamatCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<ArtifactPower>("Artifact", 2m),
        new DynamicVar("NpPerTurn", TidalCorePower.NpPerStack)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.FromPower<TidalCorePower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ArtifactPower>(choiceContext, Owner.Creature, DynamicVars["Artifact"].BaseValue, Owner.Creature, this);
        await PowerCmd.Apply<TidalCorePower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Artifact"].UpgradeValueBy(1m);
}
