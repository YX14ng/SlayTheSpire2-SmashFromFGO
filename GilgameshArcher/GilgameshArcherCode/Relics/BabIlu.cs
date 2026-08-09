using GilgameshArcher.GilgameshArcherCode.Cards;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
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
public class BabIlu : GilgameshRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel? GetUpgradeReplacement() =>
        ModelDb.Relic<EaSwordOfRupture>();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<TreasurePower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();
        await ArmsPlayedPower.EnsureInstalled(Owner.Creature);
        await TreasurePower.Seed(Owner.Creature, TreasurePower.StartingAmount);
    }
}

/// <summary>
/// Ea, la Espada de la Ruptura: la mejora Ancient de Bab-ilu. Conserva el Tesoro inicial,
/// manifiesta dos Armas al abrir el primer turno y convierte las primeras tres Armas jugadas
/// de cada turno en Carga NP adicional.
/// </summary>
public sealed class EaSwordOfRupture : BabIlu
{
    private const int StartingArms = 2;
    private const int NpPerArm = 5;
    private const int MaxProcsPerTurn = 3;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<TreasurePower>(), HoverTipFactory.FromPower<NpChargePower>()];

    public override async Task AfterPlayerTurnStart(
        PlayerChoiceContext choiceContext,
        MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player != Owner || FgoCombatState.GetCombat(Owner.Creature, 13) != 0) return;
        await FgoCombatState.SetCombat(choiceContext, Owner.Creature, 13, 1);
        Flash();
        await TreasureDeck.ManifestRandom(Owner.Creature, StartingArms);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner.Creature || cardPlay.Card is not ITreasureArm) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 10, 2) >= MaxProcsPerTurn) return;

        await FgoCombatState.IncrementTurn(
            context, Owner.Creature, 10, MaxProcsPerTurn, cardPlay.Card, width: 2);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, NpPerArm, cardPlay.Card);
    }
}
