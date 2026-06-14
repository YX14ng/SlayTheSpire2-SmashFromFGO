using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using OberonPretender.OberonPretenderCode.Powers.Forms;

namespace OberonPretender.OberonPretenderCode.Cards.Basic;

/// <summary>
/// Caer la Noche (Nightfall) -- FIRMA basica TOGGLE (DESIGN-OBERON 6.1). 1 Habilidad:
/// 4 de Bloqueo; ALTERNA entre El Rey del Cuento <-> El Principe del Invierno. La danza dia/noche desde
/// el combate 1 (patron Truco del Clan del Espejo): evita atascarse en una sola forma. En Vortigern
/// (permanente) el toggle no hace nada -- el insecto no vuelve atras.
/// </summary>
public sealed class Nightfall() : OberonCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(4m, ValueProp.Move)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);

        // Toggle Rey <-> Invierno (FormSwitch ignora el Enter si ya estas en esa forma o si la actual
        // es permanente, asi que Vortigern bloquea el toggle solo).
        if (Owner.Creature.HasPower<WinterPrincePower>())
        {
            await FormSwitch.Enter<StorybookKingPower>(choiceContext, Owner.Creature, this);
        }
        else
        {
            await FormSwitch.Enter<WinterPrincePower>(choiceContext, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(3m);
}
