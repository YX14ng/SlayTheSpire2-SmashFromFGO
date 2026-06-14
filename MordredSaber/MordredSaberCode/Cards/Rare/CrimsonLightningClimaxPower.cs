using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Rare;

/// <summary>
/// Relámpago Carmesí (Poder Clímax) (绯红闪电·巅峰之力) — DESIGN-MORDRED §5.3. 2⚡ Poder, Exhaust:
/// entrás en la forma RELÁMPAGO CARMESÍ (permanente: Ataques +2, retención 10, +5 NP/turno, sin
/// penalización; tu ulti pasa a «Interludio») (up: 1⚡). La asc 4: el clímax del eje de FORMAS, el
/// premio de fin de run. Usa FormSwitch.Enter vía la fachada Forms (FormSwitch ya bloquea reemplazos
/// de la forma permanente). El GaugeFilled de MainFile lee esta forma para manifestar el «Interludio».
/// </summary>
public sealed class CrimsonLightningClimaxPower() : MordredCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CrimsonLightningFormPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Forms.Enter<CrimsonLightningFormPower>(choiceContext, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
