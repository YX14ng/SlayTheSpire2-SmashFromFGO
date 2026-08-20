using FGOCore.FGOCoreCode.Block;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace SiegfriedSaber.SiegfriedSaberCode.Relics;

/// <summary>
/// Égida de Escamas (龙鳞战盾) — al iniciar tu turno conservás hasta 8 de Bloqueo (cap FIJO, no "todo":
/// anti-bola-de-nieve). Es el medio-arquetipo bloqueo de Siegfried (la guardia activa, complementaria a la
/// SdD persistente). Reúso de BlockRetention (toma el MAX sobre fuentes IBlockRetentionSource, nunca pelea).
///
/// <para>Parche F10 (revisión adversarial de REDESIGN-MASH-V2, 2026-08-20): esta reliquia implementaba
/// SOLO <see cref="RetentionCap"/>, o sea aportaba al cálculo del tope pero NUNCA era preventer — el
/// contrato completo de <see cref="IBlockRetentionSource"/> pide también <c>ShouldClearBlock</c> +
/// <c>AfterPreventingBlockClear</c>. Funcionaba DE PRESTADO: siempre había un <c>BulwarkPower</c>
/// cuasi-permanente que decía "no limpies" por ella. Desde que el Baluarte se GASTA cada turno
/// (REDESIGN-MASH-V2 §3 CANDADO 1), en todo turno sin carta de Baluarte no habría preventer y el
/// Bloqueo se limpiaría entero: la reliquia quedaba muerta. Ahora cumple el contrato como
/// <c>BulwarkEngineRelic</c>.</para>
/// </summary>
public sealed class DragonScaleAegis : SiegfriedRelic, IBlockRetentionSource
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private const decimal RetentionAmount = 8m;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BulwarkPower>()];

    public decimal RetentionCap(Creature creature) => creature == Owner.Creature ? RetentionAmount : 0m;

    public override bool ShouldClearBlock(Creature creature) => creature != Owner.Creature;

    public override async Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)
    {
        if (this != preventer || creature != Owner.Creature) return;
        if (creature.Block == 0) return;
        await BlockRetention.Enforce(creature);
        Flash();
    }
}
