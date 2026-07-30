using FGOCore.FGOCoreCode.Np;
using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace FGOCore.FGOCoreCode.CardTypes;

/// <summary>
/// Motor central del sistema de TIPOS DE CARTA (Buster/Arts/Quick). Power OCULTO que reciben TODOS los
/// Servants al abrir combate (lo siembra <see cref="Bond.BondRelic"/> en BeforeCombatStartLate). Su único
/// trabajo: cuando el dueño juega una carta que implementa <see cref="ICommandTyped"/>, aplicar el bonus
/// de su tipo. Una sola fuente de verdad (patrón <c>ArmsPlayedPower.AfterCardPlayed</c>): las cartas no
/// llaman a nada — solo declaran su tipo vía la interfaz.
///
/// Bonus por tipo (montos por defecto, tuneables vía las constantes de abajo):
///   Buster normal → +<see cref="BusterStrengthTemp"/> Fuerza TEMPORAL este turno (<see cref="CommandBusterStrengthPower"/>).
///   Buster ULTI   → +<see cref="BusterStrengthUlt"/> Fuerza PERMANENTE (vanilla <see cref="StrengthPower"/>).
///   Quick  normal → +<see cref="QuickStars"/> Estrellas de Crítico.
///   Quick  ULTI   → +<see cref="QuickStarsUlt"/> Estrellas de Crítico.
///   Arts   ULTI   → +<see cref="ArtsNpUlt"/> Carga NP.
///   Arts   normal → NADA aquí (las Arts de comando ya cargan NP por su propio OnPlay; no se duplica).
///
/// ORDEN: AfterCardPlayed corre DESPUÉS de resolver la carta (igual que ArmsPlayedPower/AttacksThisTurnPower),
/// así que el bonus se suma encima del efecto propio de la carta. Para Buster, la Fuerza llega después del
/// golpe de ESA carta (afecta los Ataques SIGUIENTES del turno), que es el comportamiento FGO esperado.
/// </summary>
public sealed class CommandBonusPower : FGOCorePower
{
    // --- Montos por defecto (tuneables) ---
    public const int BusterStrengthTemp = 1;  // Buster normal → +1 Fuerza temporal (este turno)
    public const int BusterStrengthUlt = 2;   // Buster ulti   → +2 Fuerza permanente
    public const int QuickStars = 10;         // Quick  normal → +10 Estrellas, tras resolver
    public const int QuickStarsUlt = 20;      // Quick  ulti   → +20 Estrellas, tras resolver
    public const int ArtsNpUlt = 20;          // Arts   ulti   → +20 Carga NP

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    protected override bool IsVisibleInternal => false;

    public override bool ShouldScaleInMultiplayer => false;

    // Power OCULTO de motor (IsVisibleInternal=false): no necesita arte propio. El icono base de
    // FGOCorePower ({id}.png) cae al fallback power.png vía PowerImagePath si no hay png dedicado.

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner?.Creature != Owner) return;
        if (cardPlay.Card is not ICommandTyped typed) return;

        await ApplyBonus(context, Owner, typed, cardPlay.Card);
    }

    /// <summary>
    /// Aplica el bonus del tipo de comando de <paramref name="typed"/> a <paramref name="creature"/>.
    /// Punto único reutilizable: el hook lo llama, pero también puede invocarse explícitamente si algún
    /// efecto necesita conceder el bonus de un tipo fuera del flujo de carta jugada. <paramref name="source"/>
    /// es la carta-origen para los helpers de recurso (puede ser null).
    /// </summary>
    public static async Task ApplyBonus(PlayerChoiceContext context, Creature creature, ICommandTyped typed, CardModel? source)
    {
        var ulti = typed.IsNoblePhantasm;
        switch (typed.CommandType)
        {
            case CommandType.Buster when ulti:
                // Fuerza PERMANENTE (vanilla StrengthPower).
                await PowerCmd.Apply<StrengthPower>(context, creature, BusterStrengthUlt, creature, source);
                break;
            case CommandType.Buster:
                // Fuerza TEMPORAL este turno (power propio FGOCore, auto-removido al fin de turno).
                await PowerCmd.Apply<CommandBusterStrengthPower>(context, creature, BusterStrengthTemp, creature, source);
                break;

            case CommandType.Quick:
                await CritStars.Gain(context, creature, ulti ? QuickStarsUlt : QuickStars, source);
                break;

            case CommandType.Arts when ulti:
                await NpCharge.Gain(context, creature, ArtsNpUlt, source);
                break;
            case CommandType.Arts:
                // Las Arts de comando ya cargan NP por su propio efecto: no se duplica.
                break;
        }
    }

    /// <summary>Garantiza el motor instalado al abrir combate (lo siembra <see cref="Bond.BondRelic"/>),
    /// para que el hook capture la PRIMERA carta tipada del combate. Idempotente.</summary>
    public static async Task EnsureInstalled(Creature creature)
    {
        await Criticals.EnsureInstalled(creature);
        if (creature.GetPower<CommandBonusPower>() == null)
        {
            await PowerCmd.Apply<CommandBonusPower>(new BlockingPlayerChoiceContext(), creature, 1m, creature, null);
        }
    }
}
