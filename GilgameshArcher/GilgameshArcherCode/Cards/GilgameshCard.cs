using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using GilgameshArcher.GilgameshArcherCode.Character;
using GilgameshArcher.GilgameshArcherCode.Extensions;

namespace GilgameshArcher.GilgameshArcherCode.Cards;

[Pool(typeof(GilgameshCardPool))]
public abstract class GilgameshCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    /// <summary>VFX por defecto de los Ataques de Gilgamesh (la lluvia de hojas de la Puerta).</summary>
    protected const string DefaultHitFx = "vfx/vfx_attack_slash";

    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    /// <summary>
    /// Ataque mono-objetivo estándar: <c>DamageCmd.Attack(amount).FromCard(this).Targeting(target)
    /// .WithHitFx(hitFx).Execute(ctx)</c>. Factoriza el bloque repetido palabra-por-palabra en ~18
    /// cartas de Gilgamesh (DESIGN-GILGAMESH §5). Comportamiento idéntico; cada carta sigue dueña de
    /// su null-check del objetivo, su cálculo de daño (base + bonus) y sus efectos posteriores
    /// (Carga NP, Estrellas, registro de Arma, etc.).
    /// </summary>
    protected Task<AttackCommand> AttackTarget(
        PlayerChoiceContext choiceContext, Creature target, decimal amount, string hitFx = DefaultHitFx) =>
        DamageCmd.Attack(amount).FromCard(this).Targeting(target)
            .WithHitFx(hitFx)
            .Execute(choiceContext);
}
