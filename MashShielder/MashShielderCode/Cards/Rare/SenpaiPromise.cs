using System.Linq;
using MashShielder.MashShielderCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace MashShielder.MashShielderCode.Cards.Rare;

/// <summary>
/// Promesa a Senpai — Power: every hit your wall stops feeds the Noble Phantasm.
/// Rediseño v2: +10 NP por golpe totalmente bloqueado (up +5 → 15; antes 6/9 —
/// denominación). Con la reliquia, cada golpe detenido paga NP y Estrellas a la vez.
/// </summary>
public sealed class SenpaiPromise() : MashShielderCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<SenpaiPromisePower>("SenpaiPromise", 10m)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<SenpaiPromisePower>(Owner.Creature, DynamicVars["SenpaiPromise"].BaseValue, Owner.Creature, this);

        // Co-op («proteger a Senpai» = cubrir a otro): cada aliado vivo recibe su propia Promesa, de
        // modo que cuando SU Bloqueo detenga un golpe, ÉL gane Carga NP (el power opera sobre su Owner).
        // En 1 jugador PlayerCreatures es solo el Owner -> el foreach queda vacío (idéntico a hoy).
        foreach (var ally in Owner.Creature.CombatState.PlayerCreatures.Where(c => c != Owner.Creature && !c.IsDead))
        {
            await PowerCmd.Apply<SenpaiPromisePower>(ally, DynamicVars["SenpaiPromise"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["SenpaiPromise"].UpgradeValueBy(5m);
    }
}
