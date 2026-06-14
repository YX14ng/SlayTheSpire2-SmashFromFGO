using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace OkitaSaber.OkitaSaberCode.Powers;

/// <summary>
/// Espíritu del Bakumatsu (幕末之魂) — al inicio de cada turno: +<see cref="BreathGain"/> *Aliento,
/// +<see cref="StarsGain"/> ★ y +<see cref="NpGainValue"/> Carga NP (DESIGN-OKITA §5.4). Engorda los
/// TRES hilos (patrón 龙之魔女). Counter (cada copia repite el paquete).
/// </summary>
public sealed class BakumatsuSpiritPower : OkitaPower
{
    public const int BreathGain = 1;
    public const int StarsGain = 10;
    public const int NpGainValue = 10;

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool ShouldScaleInMultiplayer => false;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AlientoPower>(),
        HoverTipFactory.FromPower<CritStarsPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        for (var i = 0; i < Amount; i++)
        {
            await Aliento.Gain(Owner, BreathGain, null);
            await CritStars.Gain(Owner, StarsGain, null);
            await NpCharge.Gain(Owner, NpGainValue, null);
        }
    }
}
