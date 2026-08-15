using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using ArtoriaCaster.ArtoriaCasterCode.Powers;

namespace ArtoriaCaster.ArtoriaCasterCode.Cards.Rare;

/// <summary>
/// Recarga de Hechizos (el Append 5 real) — Poder 1⚡: la primera Habilidad que jugás cada turno
/// cuesta 1⚡ menos. Mejora: las DOS primeras. Rebalance 2026-08-15 (REBALANCE-TIAMAT-ARTORIA.md
/// A7): era 2⚡ con mejora de coste — la única fuente de energía del pool era una rara cara
/// (reporte chino); ahora entra más temprano y la mejora amplía el motor en vez de abaratarlo.
/// </summary>
public sealed class SpellReloading() : ArtoriaCard(1, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<SpellReloadingPower>("Power", 1m)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SpellReloadingPower>(choiceContext, Owner.Creature, DynamicVars["Power"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1m);
    }
}
