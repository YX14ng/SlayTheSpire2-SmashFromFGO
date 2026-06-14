using GilgameshArcher.GilgameshArcherCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Basic;

/// <summary>
/// Arts / 蓝卡 (DESIGN-GILGAMESH §5.1) — FIRMA básica del mazo inicial QAABB. La carta de comando azul
/// que ENSEÑA el hilo de la Carga NP: el medidor que, a 100, manifiesta Enuma Elish solo. Un disparo
/// medido de la Puerta + batería de NP. Lleva el tag Strike heredado de la base (compat de eventos,
/// patrón Morgan/Siegfried), así que el mazo arranca sin Golpe vanilla suelto.
///
/// 6 de daño + 30 Carga NP (up +3 daño / +20 NP). Tasa: §5.1 numbers exactos. El +30 NP es el hilo
/// principal — 3-4 Arts ≈ un pico de 100 → la primera Enuma del run sin tocar oro ni armas (Plan A
/// interrumpible §3: el medidor nunca baja de la tasa base).
/// </summary>
public sealed class Arts() : GilgameshCard(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    private const int NpGain = 30;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Move),
        new DynamicVar("Np", NpGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<NpChargePower>()];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Np"].UpgradeValueBy(20m);
    }
}
