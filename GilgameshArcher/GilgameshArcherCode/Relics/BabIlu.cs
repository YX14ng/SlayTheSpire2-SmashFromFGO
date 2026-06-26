using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using GilgameshArcher.GilgameshArcherCode.Powers;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Bab-ilu, la Llave del Tesoro / 王之财宝·钥 (DESIGN-GILGAMESH §6/§7) — la starter-MOTOR de Gilgamesh:
/// la que enciende el módulo Tesoro/Armas que su pool da por sentado. Es el fallback local del (futuro)
/// módulo Arsenal/Gold de FGOCore (checklist §10). Modela la estructura de <see cref="OathOfUruk"/>
/// (GilgameshRelic, Rarity Starter, siembra powers en <c>BeforeCombatStartLate</c>).
///
/// Al iniciar cada combate:
/// - Siembra el contador de Armas (<see cref="ArmsPlayedPower.EnsureInstalled"/>) para que el hook
///   central cuente desde la PRIMERA Arma del turno (igual que OathOfUruk siembra CardsThisTurnPower).
/// - Siembra el medidor de *Tesoro (<see cref="TreasurePower.Seed"/>, 6) — el «Oro de combate» que
///   gastan los riders «Pagá X de Oro», sin tocar el oro real de la run.
///
/// La generación de Armas a la mano la hace la Puerta de Babilonia (×2 en el mazo inicial), no el
/// relic: manifestar una carta a la mano en <c>BeforeCombatStartLate</c> (antes del robo inicial) es
/// terreno no probado en el ecosistema, así que Bab-ilu sólo siembra los contadores/medidores (powers,
/// patrón probado de OathOfUruk/Fafnir/KingHassansHorn).
///
/// El Vínculo (BondRelic), Arrogancia del Rey y el contador de cartas-del-turno siguen en
/// <see cref="OathOfUruk"/>; Bab-ilu es el SEGUNDO starter, enfocado en el arsenal.
/// </summary>
public sealed class BabIlu : GilgameshRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<TreasurePower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await ArmsPlayedPower.EnsureInstalled(Owner.Creature);
        await TreasurePower.Seed(Owner.Creature, TreasurePower.StartingAmount);
    }
}
