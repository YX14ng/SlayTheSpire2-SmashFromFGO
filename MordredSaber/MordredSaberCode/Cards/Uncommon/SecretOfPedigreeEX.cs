using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MordredSaber.MordredSaberCode.Extensions;
using MordredSaber.MordredSaberCode.Powers.Forms;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Secreto de Cuna EX (不贞隐藏之兜EX) — DESIGN-MORDRED §5.2. 1⚡ Hab, Exhaust: removés TODOS tus
/// debuffs + 12 de Bloqueo + 20 NP; si Enmascarado, +10 NP más (up: 16 Bloqueo / +30 NP base), glow.
/// El S3 1:1 (cleanse + DEF + carga NP): EL CASCO mecanizado. El cleanse es uno de los DOS únicos
/// vectores permitidos del pool (regla negativa §2), vía <see cref="MordredExtensions.RemoveAllDebuffs"/>.
/// El Exhaust es el cooldown. El +10 NP de Enmascarado NO sube con el up (queda en su denominación).
/// </summary>
public sealed class SecretOfPedigreeEX() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(12m, ValueProp.Move),
        new DynamicVar("NpCharge", 20),
        new DynamicVar("MaskedNp", 10)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<MaskedKnightFormPower>(), HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Forms.InMaskedForm(Owner.Creature);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Owner.Creature.RemoveAllDebuffs();
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (Forms.InMaskedForm(Owner.Creature))
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["MaskedNp"].IntValue, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["NpCharge"].UpgradeValueBy(10m);
    }
}
