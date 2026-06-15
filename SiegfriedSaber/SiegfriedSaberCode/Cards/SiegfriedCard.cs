using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Character;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards;

[Pool(typeof(SiegfriedCardPool))]
public abstract class SiegfriedCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();

    // --- Helpers de Sangre de Dragón (SdD) compartidos por los Buster "Cazadragones" ---------------
    // Concentran el umbral mágico 3 y los caps en un único lugar (audit MODULARIZACION 2026-06-15):
    // cada carta queda con su DamageVar + una llamada, en vez de repetir el bloque condicional inline.
    // Todas son lecturas PURAS del owner (preview-safe), idénticas al código previo.

    /// <summary>Sangre de Dragón actual del dueño de la carta.</summary>
    protected int Scales => Owner.Creature.GetPowerAmount<DragonScalesPower>();

    /// <summary>Bonus plano <paramref name="bonus"/> si la SdD alcanza <paramref name="threshold"/>, si no 0
    /// (patrón "pega más mientras la armadura es gruesa": DragonbloodCut/DragonSlayerStrike/DragonHunterStrike).</summary>
    protected int ScaleThresholdBonus(int threshold, int bonus) => Scales >= threshold ? bonus : 0;

    /// <summary>Escalado acotado: +1 por SdD, tope duro <paramref name="cap"/> (HerosBackswing/DragonSlayersEdge).</summary>
    protected int CappedScaleBonus(int cap) => System.Math.Min(Scales, cap);

    /// <summary>Escalado acotado con divisor: +1 por cada <paramref name="per"/> de SdD, tope <paramref name="cap"/>
    /// (FafnirsExecutioner: +1 cada 4).</summary>
    protected int CappedScaleBonus(int per, int cap) => System.Math.Min(Scales / per, cap);

    /// <summary>True si la SdD alcanza <paramref name="threshold"/> (glow dorado de los Buster Cazadragones).</summary>
    protected bool GlowAtScales(int threshold) => Scales >= threshold;
}
