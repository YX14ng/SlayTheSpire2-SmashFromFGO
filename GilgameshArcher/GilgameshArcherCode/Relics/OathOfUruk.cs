using BaseLib.Extensions;
using BaseLib.Utils;
using FGOCore.FGOCoreCode.Stars;
using GilgameshArcher.GilgameshArcherCode.Character;
using GilgameshArcher.GilgameshArcherCode.Extensions;
using GilgameshArcher.GilgameshArcherCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Juramento del Rey de Uruk / 乌鲁克王之誓约 (DESIGN-GILGAMESH §6/§7) — el Vínculo (好感度) de Gil
/// sobre <c>BondRelic</c> de FGOCore: fuentes de puntos, umbrales y regalos por defecto (+HP/NP/Bloqueo
/// por nivel + capstone). Como <c>MashBond</c>, extiende BondRelic directo (NO la base GilgameshRelic),
/// así que re-decoramos el [Pool] y damos las rutas de icono a mano.
///
/// Overrides del diseño:
/// - <see cref="StartingNp"/>: a Nv 7 empezás cada combate con +20 de Carga NP (Append 2 real,
///   «Load Magical Energy») — pisa la curva por defecto (10/15) a partir de Nv 7.
/// - Capstone Nv 10 «El Rey de los Héroes»: empezás cada combate con 1 *Crítico Listo (el primer
///   golpe del Rey ya desprecia: próximo Ataque ×2).
///
/// PENDIENTE (módulo Arsenal de FGOCore, checklist §10): Nv 4 → empezar con 1 Arma del Tesoro extra.
/// Se engancha en <see cref="ApplyCapstone"/>/<c>BeforeCombatStartLate</c> cuando exista
/// <c>Arsenal.AddRandom</c>. No bloquea: el resto de la curva ya funciona.
/// </summary>
[Pool(typeof(GilgameshRelicPool))]
public sealed class OathOfUruk : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    // Art lives in GilgameshArcher's resources, not FGOCore's.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritReadyPower>()];

    // Append 2 (Load Magical Energy): a Nv 7+, +20 NP inicial (pisa el 10/15 por defecto).
    protected override int StartingNp(int lv) => lv >= 7 ? 20 : lv >= 5 ? 10 : lv >= 2 ? 5 : 0;

    // El contador OCULTO de cartas-del-turno (Disparo Anticipado) DEBE existir desde el primer play del
    // turno, no auto-instalarse perezosamente recién al jugar la carta (si no, "es la 1ª carta del turno"
    // daba siempre verdadero porque el power no contó las cartas previas). La starter de Gil está SIEMPRE
    // presente, así que es el lugar canónico para sembrarlo en cada combate.
    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await CardsThisTurnPower.EnsureInstalled(Owner.Creature);
    }

    // Nv 10 «El Rey de los Héroes»: el primer golpe ya es el juicio despectivo.
    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<CritReadyPower>(Owner.Creature, 1m, Owner.Creature, null);
    }
}
