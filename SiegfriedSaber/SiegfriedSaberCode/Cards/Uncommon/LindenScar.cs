using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SiegfriedSaber.SiegfriedSaberCode.Cards;
using SiegfriedSaber.SiegfriedSaberCode.Powers;

namespace SiegfriedSaber.SiegfriedSaberCode.Cards.Uncommon;

/// <summary>
/// Cicatriz del Tilo (菩提叶之伤痕 / Linden Scar) — DESIGN-SIEGFRIED §7. Poder, 1⚡:
/// cada vez que la Hoja de Tilo deja pasar un golpe (el primer golpe que te ALCANZA cada
/// turno ignora la Sangre de Dragón), +1 Sangre de Dragón y +10 Carga NP (up +15). La
/// debilidad canónica de Siegfried convertida en su MOTOR de escalado: la herida que sangra
/// engrosa la armadura y afila a Balmung (umbral SdD≥3 de la sobrecarga).
///
/// Engancha el pierce vía el listener NUEVO de FGOCore (<see cref="IDragonScalePierceListener"/>),
/// que la reliquia Hoja de Tilo dispara UNA vez por turno en el camino real del daño
/// (DragonScalesPierce.Broadcast). NO añade un piercer propio: reacciona al que ya existe.
///
/// Balance (SKILL §2/§3, techo §1.bis):
///   • Tipo Poder a 1⚡ = setup puro (§3: "carta que ocupa slot sin efecto directo" — el payoff
///     se acumula sobre el combate, no da valor el turno que se juega). Precedente de ecosistema:
///     IronWill/TirelessGuardian (Mash) = Poder 1⚡ poco común con ganancia chica por turno.
///   • Por pierce (≤1/turno, tope ESTRUCTURAL de la reliquia): +10 NP ≈ ½⚡ (§2 secundario:
///     "~10 NP ≈ 1★ ≈ ½⚡") + 1 SdD (número defensivo chico por turno, §5: pasivas 3-5/turno).
///     Suma por turno ≈ ½⚡ + un toque defensivo → POR DEBAJO de la tasa inmediata de un poco
///     común 1⚡ (11-15 daño / 11-13 bloqueo, §2) el turno en que se juega; lo "recupera" sólo
///     si la pelea dura y te PEGAN (la condición puede fallar de verdad, §3).
///   • CONDICIONAL y CAPADA: no proca en turnos sin ataque entrante ni si bloqueás del todo el
///     primer golpe; ≤1 proc/turno (P3: el trigger defensivo agregado nunca paga >2 monedas/turno
///     — acá ½⚡ + 1 SdD). Anti-batería (P2): el pierce sólo cuenta golpes que ALCANZAN (amount>0),
///     no se auto-alimenta.
///   • up +15 NP ≈ +¼⚡ por pierce (techo §1.bis tasa cruda ×1.25, no se cruza): el escalado se
///     gana aguantando golpes a lo largo del combate, nunca de un burst.
/// </summary>
public sealed class LindenScar() : SiegfriedCard(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    private const int NpPerPierce = 10; // Carga NP por pierce (up 15) — guardada en Amount del power

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new PowerVar<LindenScarPower>("Scar", NpPerPierce)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DragonScalesPower>(),
        HoverTipFactory.FromPower<NpChargePower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<LindenScarPower>(Owner.Creature, DynamicVars["Scar"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade() => DynamicVars["Scar"].UpgradeValueBy(5m);
}
