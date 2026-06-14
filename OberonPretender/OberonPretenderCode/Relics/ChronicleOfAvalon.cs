using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using OberonPretender.OberonPretenderCode.Character;
using OberonPretender.OberonPretenderCode.Extensions;
using OberonPretender.OberonPretenderCode.Powers;

namespace OberonPretender.OberonPretenderCode.Relics;

/// <summary>
/// Cronica de Avalon le Fae (妖精国阿瓦隆编年史 / Chronicle of Avalon le Fae) — reliquia STARTER de
/// vinculo (DESIGN-OBERON §7 #2) sobre el <see cref="BondRelic"/> de FGOCore. Espina numerica default
/// (Max HP a Nv1/3/6/9, +Bloqueo a Nv4/7) con dos giros de Oberon:
/// - Nv2/5/8: empezas el combate con 5/10/15 NP (default del Bond) PERO los escalones altos suman
///   tambien Estrellas (Nv7: +20★ extra; el doping de la noche que arranca al amanecer).
/// - Capstone Nv10 «El Final Feliz» (幸福结局): al iniciar cada combate +2 de Bendicion de Sobrecarga
///   (<see cref="OverchargeBlessingPower"/>) — empuja las Desatadas a >=150 desde el turno 1.
///
/// El ×1.25 global de daño/bloqueo NO existe (el Bond del ecosistema ya lo elimino — la potencia de
/// Oberon viene de sus motores gateados, no de una tasa plana; ver BondRelic.cs y la regla §1.bis.2).
/// El arte vive en los recursos de Oberon, no en FGOCore (patron MorganBond).
/// </summary>
[Pool(typeof(OberonRelicPool))]
public sealed class ChronicleOfAvalon : BondRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    private const int CapstoneBlessing = 2; // Nv10: empuja las ultis a >=150
    private const int Lv7BonusStars = 20;   // el escalon alto añade Estrellas al despertar

    // El arte vive en los recursos de Oberon (no en FGOCore) — patron MorganBond.
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromPower<NpChargePower>(), HoverTipFactory.FromPower<CritStarsPower>()];

    public override async Task BeforeCombatStartLate()
    {
        await base.BeforeCombatStartLate();

        // Nv7+: el despertar trae ademas Estrellas (el doping de la noche).
        if (Level >= 7)
        {
            await CritStars.Gain(Owner.Creature, Lv7BonusStars, null);
        }
    }

    protected override async Task ApplyCapstone()
    {
        await PowerCmd.Apply<OverchargeBlessingPower>(Owner.Creature, CapstoneBlessing, Owner.Creature, null);
    }
}
