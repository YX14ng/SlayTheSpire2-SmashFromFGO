using FGOCore.FGOCoreCode.Lahmu;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace TiamatBeast.TiamatCode.Powers;

/// <summary>
/// Vigilia de la Madre — motor de robo condicionado a la población: al inicio de TUS turnos, si tenés
/// 3+ Laḫmu en el campo, robás 1 carta por acumulación. El único motor de robo persistente del pool
/// (solo Amamantar/Llamado roban, y una vez); la condición ata el robo al plan de enjambre, así la
/// carta es mediocre sin el motor y fuerte con él (§1.4 del skill). La concede la carta poco común
/// <c>MothersVigil</c>. Apilable (Counter).
/// </summary>
public sealed class MothersVigilPower : TiamatPower
{
    public const int LahmuThreshold = 3;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<LahmuSwarmPower>()];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || Owner.Player == null) return;
        if (Lahmu.Count(Owner) < LahmuThreshold) return;
        Flash();
        await CardPileCmd.Draw(choiceContext, Amount, Owner.Player);
    }
}
