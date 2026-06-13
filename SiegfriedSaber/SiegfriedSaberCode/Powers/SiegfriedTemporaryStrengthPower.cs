using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

namespace SiegfriedSaber.SiegfriedSaberCode.Powers;

/// <summary>
/// Subclase concreta de la vanilla abstracta <see cref="TemporaryStrengthPower"/> (no se puede
/// PowerCmd.Apply&lt;&gt; sobre una clase abstracta). Da Fuerza TEMPORAL este turno y se auto-remueve
/// en AfterTurnEnd (toda la lógica vive en la base). La usa el Rank-Up de Cazador A++ (DragonHunterStrike).
/// OriginModel = la carta (patrón vanilla AnticipatePower/CoordinatePower: la base usa OriginModel sólo
/// para el Título/HoverTip, que toma del CardModel — devolver el power tiraría InvalidOperationException).
/// </summary>
public sealed class SiegfriedTemporaryStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Card<DragonHunterStrike>();
}
