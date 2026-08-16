using KagetoraLancer.KagetoraLancerCode.Doctrine;
using KagetoraLancer.KagetoraLancerCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace KagetoraLancer.KagetoraLancerCode.Relics;

public sealed class SakeCup : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.SakeCup) || player != Owner) return;
        await KagetoraUsages.Mark(context, Owner.Creature, KagetoraUsage.SakeCup, null);
        await CardPileCmd.Draw(context, 1, Owner);
        var selected = await CardSelectCmd.FromHandForDiscard(context, Owner,
            new CardSelectorPrefs(SelectionScreenPrompt, 1, 1), null, this);
        await CardCmd.Discard(context, selected);
    }
}

public sealed class EightPetalBanner : KagetoraRelic, IDoctrineAdvanceListener
{
    public override RelicRarity Rarity => RelicRarity.Common;

    public async Task AfterDoctrineAdvance(PlayerChoiceContext context, DoctrineAdvance result)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.EightPetalBanner) ||
            !result.Advanced) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.EightPetalBanner, result.CardPlay.Card);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 2m, ValueProp.Unpowered, null);
    }
}

/// <summary>
/// §8: la primera Pies de cada turno, +10★.
/// <para>§16.6 (colisión de re-tipado, auditada y ACEPTADA): con el pool nuevo la primera Pies del
/// turno puede ser <c>StarlitCharge</c>, que es un Ataque de <b>0⚡</b>. La reliquia se dispara igual
/// —el precepto no cambió— pero deja de costar energía activarla. Aceptado: la conversión ya paga
/// 20★ y el neto de esa carta sigue siendo negativo en estrellas (E5).</para>
/// </summary>
public sealed class HoushoutsukigeReins : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.HoushoutsukigeReins) ||
            cardPlay.Card.Owner != Owner || cardPlay.Card is not IPreceptCard { Precept: Precept.Feet }) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.HoushoutsukigeReins, cardPlay.Card);
        Flash();
        await CritStars.Gain(context, Owner.Creature, 10, cardPlay.Card);
    }
}

/// <summary>
/// §8: la primera <b>carta de Pecho</b> de cada turno, +4 Bloqueo.
/// <para>§16.6 (delta silencioso declarado, va al changelog): el guard mira el PRECEPTO, no el tipo,
/// así que ahora también la disparan las dos comunes nuevas de Pecho que son <b>Ataques</b>
/// (<c>SpearWall</c>, <c>EchigoRampart</c>). Es coherente con el texto —dice «carta de Pecho»— y
/// deliberado: Pecho dejó de ser el único precepto sin Ataques. Se verificó que
/// <c>FearlessChestPower</c> NO colisiona: escucha el AVANCE, no el tipo de carta.</para>
/// </summary>
public sealed class SixPlateArmour : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.SixPlateArmour) ||
            cardPlay.Card.Owner != Owner || cardPlay.Card is not IPreceptCard { Precept: Precept.Chest }) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.SixPlateArmour, cardPlay.Card);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, 4m, ValueProp.Unpowered, null);
    }
}

public sealed class ShiranuiTachi : KagetoraRelic, ICriticalConsumedListener
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    public async Task AfterCriticalConsumed(PlayerChoiceContext context, CriticalHit critical)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.ShiranuiTachi) ||
            critical.Owner != Owner.Creature) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.ShiranuiTachi, critical.Card);
        Flash();
        await NpCharge.Gain(context, Owner.Creature, 10, critical.Card);
    }
}

/// <summary>
/// §8 / §5: al ascender, <b>+1⚡ este turno, +30★ y +50 de Carga NP</b>. Es el arranque de la segunda
/// carrera de NP (hoy la ascensión dejaba el medidor en 0 y había que juntar 100 desde cero), y es un
/// pico ACOTADO A UN TURNO, no un techo nuevo: por eso Kenshin no recibe +1⚡ permanente (§12.3-3).
/// </summary>
public sealed class WhiteFlameBrazier : KagetoraRelic, IAscensionListener
{
    public const int AscensionEnergy = 1;
    public const int AscensionStars = 30;
    public const int AscensionNpCharge = 50;

    public override RelicRarity Rarity => RelicRarity.Rare;
    public async Task AfterAscendingToKenshin(PlayerChoiceContext context, CardModel source)
    {
        if (KagetoraUsages.WasUsed(Owner.Creature, KagetoraUsage.WhiteFlameBrazier)) return;
        await KagetoraUsages.Mark(
            context, Owner.Creature, KagetoraUsage.WhiteFlameBrazier, source);
        Flash();
        // §16.2 advierte contra colgar ganancia de energía de AfterSideTurnStart (corre antes o
        // después del reset según el orden interno) y manda AfterEnergyReset para las ganancias POR
        // TURNO. Acá no aplica: la ascensión ocurre a mitad de turno, al resolver el NP, y «+1⚡ este
        // turno» significa AHORA. Un AfterEnergyReset daría la energía recién al turno siguiente, que
        // es otro turno. PlayerCmd.GainEnergy directo es el camino correcto para este disparador.
        await PlayerCmd.GainEnergy(AscensionEnergy, Owner);
        await CritStars.Gain(context, Owner.Creature, AscensionStars, source);
        await NpCharge.Gain(context, Owner.Creature, AscensionNpCharge, source);
    }
}

public sealed class EchigoSaltBag : KagetoraRelic
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    public override async Task BeforeCombatStartLate()
    {
        var players = Owner.Creature.CombatState?.PlayerCreatures;
        if (players == null) return;
        foreach (var creature in players)
            await PowerCmd.Apply<ArtifactPower>(new BlockingPlayerChoiceContext(), creature, 1m, Owner.Creature, null);
        var allies = Math.Max(0, players.Count - 1);
        if (allies > 0) await NpCharge.Gain(Owner.Creature, allies * 10, null);
    }
}
