using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MordredSaber.MordredSaberCode.Extensions;

namespace MordredSaber.MordredSaberCode.Cards.Uncommon;

/// <summary>
/// Desdén al Trono (蔑视王座) — DESIGN-MORDRED §5.2. 1⚡ Hab: +20 de Carga NP; vs Élite/Jefe +10 más
/// (up +10 NP base), glow. El anti-autoridad reciclado a NP (su desdén por quien se sienta más alto).
/// Leído con <see cref="MordredExtensions.VersusAuthority"/>. El +10 condicional NO sube con el up.
/// Patrón ScornForTheThrone (NP-feed con rider anti-autoridad).
/// </summary>
public sealed class ScornForTheThrone() : MordredCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("NpCharge", 20), new DynamicVar("Authority", 10)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override bool ShouldGlowGoldInternal => Owner.Creature.VersusAuthority();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await NpCharge.Gain(Owner.Creature, DynamicVars["NpCharge"].IntValue, this);
        if (Owner.Creature.VersusAuthority())
        {
            await NpCharge.Gain(Owner.Creature, DynamicVars["Authority"].IntValue, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars["NpCharge"].UpgradeValueBy(10m);
}
