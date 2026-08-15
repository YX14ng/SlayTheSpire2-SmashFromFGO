using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using TiamatBeast.TiamatCode.Powers;

namespace TiamatBeast.TiamatCode.Cards.Uncommon;

/// <summary>
/// Limo Protector — Poder poco común: concede <see cref="ProtectiveSiltPower"/> (+4 Bloqueo al inicio
/// de tus turnos por acumulación). El Metallicize de Tiamat. Rebalance 2026-08-15: era Baluarte
/// (3/turno retenido que componía el piso defensivo combate entero); ahora Bloqueo plano 4/turno
/// (+1 de compensación por perder la retención). También suma al hueco Poco común/Poder=1 que
/// rompía el evento de pociones.
/// </summary>
public sealed class ProtectiveSilt() : TiamatCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Block", 4)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<ProtectiveSiltPower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ProtectiveSiltPower>(choiceContext, Owner.Creature, DynamicVars["Block"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(2m);
}
