using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Rugido de Rebelión (叛逆咆哮) — DESIGN-MORDRED §5.2. 0⚡ Hab: cambiá a tu forma OPUESTA + robá 1
/// (up: robá 2). El pan-y-manteca de FORMAS: si estás Enmascarada te arrancás el yelmo (Rebelión); si
/// estás en Rebelión te lo ponés (Enmascarado). En Relámpago Carmesí (permanente) el switch es no-op
/// (FormSwitch lo bloquea), pero igual robás. Patrón SwitchOrtinax + robo.
/// </summary>
public sealed class RoarOfRebellion() : MordredCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<RebellionFormPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Forms.InMaskedForm(Owner.Creature))
        {
            await Forms.Enter<RebellionFormPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await Forms.Enter<MaskedKnightFormPower>(choiceContext, Owner.Creature, this);
        }
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1m);
}
