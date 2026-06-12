using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Recarga de Hechizos (el Append 5 real) — Poder 2⚡: la PRIMERA Habilidad que
/// jugás cada turno cuesta 1⚡ menos (hook TryModifyEnergyCostInCombat en
/// SpellReloadingPower). Mejora: coste 1⚡.
/// </summary>
public sealed class SpellReloading() : ArtoriaCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SpellReloadingPower>("Power", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SpellReloadingPower>(Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
