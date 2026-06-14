using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MordredSaber.MordredSaberCode.Cards.Uncommon;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// Subclase concreta de la vanilla abstracta <see cref="TemporaryStrengthPower"/> (no se puede
/// PowerCmd.Apply<> sobre una clase abstracta). Da Fuerza TEMPORAL este turno y se auto-remueve
/// en AfterTurnEnd (toda la lógica vive en la base). La usa «Estallido de Maná A» (Mana Burst A): el
/// S1 base 1:1, este turno tus Ataques hacen +4 (up +6) con Exhaust como cooldown. Patrón
/// SiegfriedTemporaryStrengthPower (DragonHunterStrike). OriginModel = la carta (la base usa
/// OriginModel sólo para el Título/HoverTip que toma del CardModel; devolver el power tiraría).
/// </summary>
public sealed class ManaBurstStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<ManaBurstA>();
}
