using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Relics;

/// <summary>
/// Manto de la Arrogancia (傲慢之披风) — reliquia POCO COMÚN (DESIGN-GILGAMESH §6). Cada vez que un ataque
/// tuyo con Crítico Listo mata a un enemigo: ganás 5 de Oro (máx 15/combate). El juicio cobra botín; el
/// cap evita el farmeo. Patrón ThreatToHumanityPower (<c>result.WasTargetKilled</c> + dealer == dueño) con
/// el flag por combate reseteado en <see cref="BeforeCombatStartLate"/>.
/// </summary>
public sealed class MantleOfArrogance : GilgameshRelic
{
    private const int GoldPerKill = 5;
    private const int MaxGoldPerCombat = 15;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    private int _goldThisCombat;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new GoldVar(GoldPerKill)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public override Task BeforeCombatStartLate()
    {
        _goldThisCombat = 0;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (dealer != Owner.Creature || !props.IsPoweredAttack() || !result.WasTargetKilled) return;
        if (!Owner.Creature.HasPower<CritReadyPower>()) return;
        if (_goldThisCombat >= MaxGoldPerCombat) return;

        _goldThisCombat += GoldPerKill;
        Flash();
        await PlayerCmd.GainGold(DynamicVars.Gold.BaseValue, Owner);
    }
}
