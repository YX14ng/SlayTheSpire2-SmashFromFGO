using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Égida de Escamas (龙鳞战盾) — al iniciar tu turno conservás hasta 8 de Bloqueo (cap FIJO, no "todo":
/// anti-bola-de-nieve). Es el medio-arquetipo bloqueo de Siegfried (la guardia activa, complementaria a la
/// SdD persistente). Reúso de BlockRetention (toma el MAX sobre fuentes IBlockRetentionSource, nunca pelea).
/// </summary>
public sealed class DragonScaleAegis : SiegfriedRelic, IBlockRetentionSource
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const decimal RetentionAmount = 8m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    public decimal RetentionCap(Creature creature) => creature == Owner.Creature ? RetentionAmount : 0m;
}
