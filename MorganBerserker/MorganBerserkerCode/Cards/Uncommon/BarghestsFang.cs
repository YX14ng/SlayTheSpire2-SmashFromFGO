using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Uncommon;

/// <summary>
/// Colmillo de Barghest (巴格斯特之牙) — 16 de daño; si el objetivo tiene Maldición:
/// cura 4 HP. El bono (cura) depende del OBJETIVO elegido, no del estado global: sin glow
/// dorado, porque la carta no puede saber a qué enemigo apuntarás (un glow por "hay algún
/// maldito" mentiría al apuntar a un enemigo sin Maldición). Auditoría 2026-06-15.
/// </summary>
// IUsesTargetCurse: la Sentencia (forma Reina/Invierno) consumía la Maldición del objetivo en
// BeforeCardPlayed antes de este OnPlay -> wasCursed=false -> no curaba (reporte de player). El marcador
// exime la carta de la Sentencia y le deja la Maldición para leerla.
public sealed class BarghestsFang() : MorganCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IUsesTargetCurse
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new HealVar(3m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var wasCursed = Curses.Of(cardPlay.Target) > 0;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_bloody_impact")
            .Execute(choiceContext);
        if (wasCursed)
        {
            await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars.Heal.UpgradeValueBy(1m);
    }
}
