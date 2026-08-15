using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace MorganBerserker.MorganBerserkerCode.Relics;

/// <summary>
/// Espejo del Clan (镜之氏族的魔镜) — re-efecto RE-POOL V2 (parche J1-2, de P2): la primera vez
/// que cambiás de forma cada turno: 3 de Bloqueo. El robo por cambio SIN cap quedó prohibido
/// (con el toggle común nuevo + cetro re-armable era un motor de robo gratis — falla compartida
/// de P1/P3). Flag en el bit 11 del estado de turno.
/// </summary>
public sealed class MirrorClanGlass : MorganRelic, IFormChangeListener
{
    public const int BlockPerSwitch = 3;

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public async Task OnFormChanged(PlayerChoiceContext? choiceContext)
    {
        if (FgoCombatState.GetTurn(Owner.Creature, 11) != 0) return;
        await FgoCombatState.SetTurn(
            choiceContext ?? new BlockingPlayerChoiceContext(), Owner.Creature, 11, 1);
        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, BlockPerSwitch, ValueProp.Unpowered, null);
    }
}
