using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Corona de Espinas (荆棘王冠) — RE-POOL V2 [NUEVA] (aprobada J2-18): la primera vez POR TURNO
/// que una CARTA te hace perder HP: ganás 4 de Bloqueo. Sustain capado de la línea D que no borra
/// su debilidad (los golpes enemigos no cuentan — solo el precio que la tirana paga por voluntad).
/// Detección: pérdida con cardSource propio; el tick de FaeBloodPact (power, sin carta) no
/// dispara. Flag en el bit 10 del estado de turno.
/// </summary>
public sealed class CrownOfThorns : MorganRelic
{
    public const int BlockOnSelfHarm = 4;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (!CombatManager.Instance.IsInProgress || target != Owner.Creature || result.UnblockedDamage <= 0) return;
        if (cardSource?.Owner?.Creature != Owner.Creature) return;
        if (FgoCombatState.GetTurn(Owner.Creature, 10) != 0) return;

        await FgoCombatState.SetTurn(choiceContext, Owner.Creature, 10, 1, cardSource);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, BlockOnSelfHarm, ValueProp.Unpowered, null);
    }
}
