using FGOCore.FGOCoreCode.DragonScales;
using FGOCore.FGOCoreCode.Np;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Basic;

/// <summary>
/// Bautismo de Sangre (浴血之躯) — firma básica de CONSTRUCCIÓN (DESIGN-SIEGFRIED §5: "una que
/// construya SdD o NP de forma básica"). Enseña LAS DOS identidades de recurso de un solo gesto:
/// la armadura (Sangre de Dragón) y el medidor (Carga NP). Bañarse en la sangre del dragón
/// engrosa la piel y carga el filo.
///
/// Bloqueo 5 + 1 Sangre de Dragón (permanente) + 8 NP. Es un turno-tanque honesto: NO pega, pero
/// sube el SdD para cruzar el umbral ≥3 que afila al Tajo Cazadragones y a Balmung (la lente del
/// diseño: el escalado se GANA aguantando la armadura). El SdD NO es bloqueo: persiste, reduce cada
/// golpe ENTRANTE, y la Hoja de Tilo deja pasar igual el primer golpe que te alcanza.
///
/// Balance (skill SKILL §2): 1⚡ Común bloqueo puro = 7-9; acá Bloqueo 5 porque CARGA dos riders
/// (§3 "1⚡ + rider" baja a 4-7). NP: SKILL §2 "~10 NP ≈ ½⚡", damos 8 (un pelo bajo el medio-⚡ por
/// ir grapado al bloqueo). +1 SdD = rider permanente chico (~+2-3 de efecto a lo largo del combate,
/// acotado por el pierce 1/turno de la Hoja de Tilo). No hay loop AFK: NO genera por golpe recibido
/// (eso es la Hoja de Tilo, con su tope P2/P3), sólo al JUGARLA. El jugador empuja, no acampa.
/// </summary>
public sealed class BloodBaptism() : SiegfriedCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    private const int NpGain = 8;   // SKILL §2: ~10 NP ≈ ½⚡; 8 por ir grapado al bloqueo
    private const int ScalesGain = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("Scales", ScalesGain),
        new DynamicVar("Np", NpGain)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DragonScalesPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, (BlockVar)DynamicVars.Block, cardPlay);
        await PowerCmd.Apply<DragonScalesPower>(Owner.Creature, DynamicVars["Scales"].IntValue, Owner.Creature, this);
        await NpCharge.Gain(Owner.Creature, DynamicVars["Np"].IntValue, this);
    }

    // Up (Rank-Up-as-upgrade, §6 estilo): +2 bloqueo y +4 NP (no toca el SdD: el +1 permanente
    // ya escala solo con la repetición; subir el SdD por carta rompería el techo del pierce).
    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["Np"].UpgradeValueBy(4m);
    }
}
