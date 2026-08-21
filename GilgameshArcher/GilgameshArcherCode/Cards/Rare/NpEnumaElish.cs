using GilgameshArcher.GilgameshArcherCode.Cards;
using GilgameshArcher.GilgameshArcherCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace GilgameshArcher.GilgameshArcherCode.Cards.Rare;

/// <summary>
/// NP Enuma Elish (天地乖离开辟之星) — DESIGN-GILGAMESH §5.4. La carta-NP DRAFTEABLE: disparar Enuma ANTES
/// del auto-ulti. 2⚡, Exhaust; mín. 70, consume TODA la carga (<c>ConsumeAllForNpCard</c>); marca
/// <see cref="IGilgameshNpCard"/>.
///
/// 24 de daño a TODOS los enemigos, +2 por cada 10 de carga consumida sobre 70 (la Sobrecarga, a TODOS);
/// contra Élites/Jefes (<see cref="RoyalTrait.IsDivine"/>) +12 adicional. El daño base + sobrecarga escala
/// con dupes (<see cref="NpLevels.Scale"/>). La <c>OverchargeBlessingPower</c> ya está horneada en
/// <c>ConsumeAllForNpCard</c> (sube el tier consumido antes de calcular la sobrecarga). up: 30 base /
/// +15 anti-divino. Glow cuando es pagable.
/// </summary>
public sealed class NpEnumaElish() : GilgameshCard(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies), IGilgameshNpCard, ICommandTyped
{
    CommandType ICommandTyped.CommandType => CommandType.Buster;
    public bool IsNoblePhantasm => true;

    public const int ChargeCost = 70;

    private const int PerTen = 2; // +2 a TODOS por cada 10 de carga consumida sobre 70

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new DynamicVar("Divine", 12),
        new DynamicVar("ChargeCost", ChargeCost)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<NpChargePower>(),
        HoverTipFactory.FromPower<OverchargeBlessingPower>()
    ];

    protected override bool IsPlayable => NpCharge.CanPay(Owner.Creature, ChargeCost, this);

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Misma red de seguridad que ENUMA ELISH: Desatado — ver el comentario largo alli. Resumen:
        // la carta jugada vive en el centro de la pantalla hasta que OnPlay RETORNA, asi que una
        // excepcion en esta cadena la deja congelada ahi (el sintoma que reportaron dos jugadores).
        var phase = "carga";
        try
        {
            // 1) Consume TODA la carga; tier = lo realmente consumido (>= 70, + OverchargeBlessing).
            var tier = await NpCharge.ConsumeAllForNpCard(choiceContext, Owner.Creature, ChargeCost, this);
            var overcharge = (tier - ChargeCost) / 10 * PerTen; // sobrecarga a TODOS

            // 2) base + sobrecarga es plano para todos; solo Elites/Jefes reciben +Divine.
            //    La animacion de ataque se dispara UNA sola vez para todo el NP (igual que la ulti
            //    auto-manifestada): un Execute por enemigo la repetia N veces, con su espera cada vez.
            phase = "dano";
            var combatState = Owner.Creature.CombatState;
            if (combatState == null) return;

            var animPlayed = false;
            foreach (var enemy in combatState.GetOpponentsOf(Owner.Creature).ToList())
            {
                if (enemy.IsDead) continue;
                var divineBonus = RoyalTrait.IsDivine(enemy) ? DynamicVars["Divine"].IntValue : 0;
                var damage = NpLevels.Scale(Owner, DynamicVars.Damage.BaseValue + overcharge + divineBonus);
                var attack = DamageCmd.Attack(damage).FromCardFgoCompatibility(this, cardPlay).Targeting(enemy)
                    .WithHitFx("vfx/vfx_starry_impact");
                if (animPlayed) attack = attack.WithNoAttackerAnim();
                animPlayed = true;
                await attack.Execute(choiceContext);
            }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error(
                $"NP Enuma Elish aborto en la fase '{phase}'. La carta termina su resolucion igual " +
                $"para no quedar congelada en el centro de la pantalla. {ex}");
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(6m);
        DynamicVars["Divine"].UpgradeValueBy(3m);
    }
}
