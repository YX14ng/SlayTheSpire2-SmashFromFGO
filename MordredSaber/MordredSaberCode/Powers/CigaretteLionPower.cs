using System.Linq;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace MordredSaber.MordredSaberCode.Powers;

/// <summary>
/// León del Cigarrillo B+ (香烟雄狮B+, §5.2) — el Rank-Up de Instinto B como PODER (guiño a Kairi
/// Sisigou). DESIGN-MORDRED §5.2: cada vez que OBTENÉS un *Crítico Listo* (FGOCore
/// <see cref="CritReadyPower"/> sube — típicamente el auto-proc de ★ a 100), robás 1 carta. El
/// +20★ inicial lo da la carta al jugarse (no el poder). Detección por el mismo hook que usa
/// CritStarsPower (<see cref="AfterPowerAmountChanged"/>): cuando el power que cambió es un
/// Crítico Listo del owner y subió (amount > 0), robamos. Counter: las copias apilan el robo.
/// El up de la carta baja el costo a 1⚡ (no toca el robo). Personal: no escala en multijugador.
/// </summary>
public sealed class CigaretteLionPower : MordredPower, ICriticalConsumedListener
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<CritReadyPower>()];

    public async Task AfterCriticalConsumed(PlayerChoiceContext choiceContext, CriticalHit critical)
    {
        if (critical.Owner != Owner) return;
        var player = Owner.Player;
        if (player?.PlayerCombatState is not { } playerCombatState || Owner.IsDead) return;
        Flash();
        // BUGFIX (soft-lock): este robo proca a MITAD de la resolución de la carta que disparó el
        // Crítico Listo. Si RESHUFFLEA (mazo vacío), reshufflea el descarte -que en v0.107.1 contiene
        // la carta en curso- y corrompe su estado ("must be added to a CombatState"), colgando el
        // combate. Por eso robamos SOLO lo que hay en el mazo (sin gatillar reshuffle).
        var inDeck = playerCombatState.AllPiles
            .FirstOrDefault(p => p.Type == PileType.Draw)?.Cards.Count ?? 0;
        var toDraw = System.Math.Min((int)Amount, inDeck);
        if (toDraw > 0)
        {
            await CardPileCmd.Draw(choiceContext, toDraw, player);
        }
    }
}
