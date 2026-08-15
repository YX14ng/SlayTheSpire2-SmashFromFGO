using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Cards.Common;

/// <summary>#3 Desdén de la Reina (女王的轻蔑) — 4 de daño; +3 si el objetivo tiene Maldición.
/// El bono depende del OBJETIVO elegido, no del estado global: sin glow dorado (la carta
/// no puede saber a qué enemigo apuntarás, así que un glow por "hay algún maldito" mentiría
/// cuando apuntás a un enemigo sin Maldición). Auditoría 2026-06-15.</summary>
// IUsesTargetCurse: exime de la Sentencia (que consumía la Maldición del objetivo antes del OnPlay),
// para que el +daño condicional a "objetivo maldito" dispare en forma Reina/Invierno.
public sealed class QueensScorn() : MorganCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy), IUsesTargetCurse
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
        new DynamicVar("Bonus", 5)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CursePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var damage = DynamicVars.Damage.BaseValue;
        if (Curses.Of(cardPlay.Target) > 0)
        {
            damage += DynamicVars["Bonus"].BaseValue;
        }
        await DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Bonus"].UpgradeValueBy(2m);
    }
}
