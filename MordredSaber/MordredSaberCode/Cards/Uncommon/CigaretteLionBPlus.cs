using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Powers;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// León del Cigarrillo B+ (香烟雄狮B+) — DESIGN-MORDRED §5.2. 2⚡ Poder (up: 1⚡): al jugarla +20
/// Estrellas; cada vez que OBTENÉS un *Crítico Listo*, robá 1 (guiño a Kairi Sisigou). El Rank-Up
/// de Instinto B como Poder. El +20★ es inmediato; el robo recurrente lo gestiona
/// <see cref="CigaretteLionPower"/> (engancha el alza de CritReadyPower). El up baja el costo a 1⚡
/// (EnergyCost.UpgradeBy, patrón SpellReloading), no toca los números.
/// </summary>
public sealed class CigaretteLionBPlus() : MordredCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Stars", 20)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<CritStarsPower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CritStars.Gain(Owner.Creature, DynamicVars["Stars"].IntValue, this);
        await PowerCmd.Apply<CigaretteLionPower>(Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
