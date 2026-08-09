using FGOCore.FGOCoreCode.Np;
using FGOCore.FGOCoreCode.Stars;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;

namespace FGOCore.FGOCoreCode.Ritsu;

/// <summary>
/// Publica Carga NP y Estrellas como recursos secundarios de RitsuLib. Durante la migracion, los
/// powers historicos siguen siendo la representacion autoritativa y serializable para no invalidar
/// partidas guardadas; este puente mantiene ambas superficies sincronizadas en los dos sentidos.
/// </summary>
public static class FgoSecondaryResources
{
    public const string NpChargeLocalId = "NP_CHARGE";
    public const string CritStarsLocalId = "CRIT_STARS";
    public const string NpChargeResourceId = "FGO_CORE_SECONDARY_RESOURCE_NP_CHARGE";
    public const string CritStarsResourceId = "FGO_CORE_SECONDARY_RESOURCE_CRIT_STARS";

    private static readonly object Sync = new();
    private static readonly HashSet<(Player Player, string ResourceId)> ActiveBridges = [];
    private static bool _initialized;

    public static void Initialize()
    {
        lock (Sync)
        {
            if (_initialized) return;

            var registry = RitsuLibFramework.GetSecondaryResourceRegistry(MainFile.ModId);
            var np = registry.Register(
                NpChargeLocalId,
                new SecondaryResourceDefinition(
                    baseMaxAmount: NpChargePower.Max,
                    hardMaxAmount: int.MaxValue,
                    persistencePolicy: SecondaryResourcePersistencePolicy.Combat,
                    locTable: "powers",
                    titleKey: "FGOCORE-NP_CHARGE_POWER.title",
                    descriptionKey: "FGOCORE-NP_CHARGE_POWER.description",
                    smallIconPath: $"{MainFile.ResPath}/images/powers/np_charge_power.png",
                    largeIconPath: $"{MainFile.ResPath}/images/powers/big/np_charge_power.png"));
            var stars = registry.Register(
                CritStarsLocalId,
                new SecondaryResourceDefinition(
                    baseMaxAmount: CritStarsPower.Max,
                    hardMaxAmount: CritStarsPower.Max,
                    persistencePolicy: SecondaryResourcePersistencePolicy.Combat,
                    locTable: "powers",
                    titleKey: "FGOCORE-CRIT_STARS_POWER.title",
                    descriptionKey: "FGOCORE-CRIT_STARS_POWER.description",
                    smallIconPath: $"{MainFile.ResPath}/images/powers/crit_stars_power.png",
                    largeIconPath: $"{MainFile.ResPath}/images/powers/big/crit_stars_power.png"));

            ValidateStableId(np, NpChargeResourceId);
            ValidateStableId(stars, CritStarsResourceId);
            SecondaryResourceHook.RegisterGlobalListener(new LegacyPowerBridge());
            RitsuLibFramework.SubscribeLifecycle<CombatStartingEvent>(OnCombatStarting, replayCurrentState: false);

            _initialized = true;
            MainFile.Logger.Info(
                $"RitsuLib recursos FGO registrados: {NpChargeResourceId}, {CritStarsResourceId}.");
        }
    }

    /// <summary>Muestra ambos medidores de RitsuLib incluso cuando el personaje empieza en cero.</summary>
    public static void RegisterCharacter<TCharacter>() where TCharacter : CharacterModel
    {
        Initialize();
        var registry = RitsuLibFramework.GetSecondaryResourceRegistry(MainFile.ModId);
        registry.AlwaysShowInCombatUiForCharacter<TCharacter>(NpChargeLocalId);
        registry.AlwaysShowInCombatUiForCharacter<TCharacter>(CritStarsLocalId);
    }

    /// <summary>
    /// Reconstruye la superficie RitsuLib desde los powers conservados. Se ejecuta al abrir o
    /// reanudar cada combate, despues de que el juego restauro los powers del jugador.
    /// </summary>
    public static async Task SynchronizeFromLegacyPowers(Creature creature)
    {
        await SynchronizeFromLegacyPower(creature, NpChargeResourceId, NpCharge.Current(creature));
        await SynchronizeFromLegacyPower(creature, CritStarsResourceId, CritStars.Of(creature));
    }

    private static void OnCombatStarting(CombatStartingEvent evt)
    {
        if (evt.CombatState == null) return;

        foreach (var player in evt.CombatState.Players)
        {
            var creature = player.Creature;
            if (creature == null) continue;

            // SecondaryResourceCmd actualiza el estado antes de su primer await. Observamos las
            // tareas por separado para sembrar ambos recursos al inicio sin bloquear el hilo Godot.
            _ = ObserveCombatStartSync(SynchronizeNpFromLegacy(creature));
            _ = ObserveCombatStartSync(SynchronizeStarsFromLegacy(creature));
        }
    }

    private static async Task ObserveCombatStartSync(Task synchronization)
    {
        try
        {
            await synchronization;
        }
        catch (Exception exception)
        {
            MainFile.Logger.ErrorNoTrace(
                $"No se pudo reconstruir un recurso FGO al iniciar el combate: {exception}");
        }
    }

    internal static async Task SynchronizeNpFromLegacy(Creature creature, AbstractModel? source = null)
    {
        await SynchronizeFromLegacyPower(creature, NpChargeResourceId, NpCharge.Current(creature), source);
    }

    internal static async Task SynchronizeStarsFromLegacy(Creature creature, AbstractModel? source = null)
    {
        await SynchronizeFromLegacyPower(creature, CritStarsResourceId, CritStars.Of(creature), source);
    }

    private static async Task SynchronizeFromLegacyPower(
        Creature creature,
        string resourceId,
        int amount,
        AbstractModel? source = null)
    {
        var player = creature.Player;
        if (player == null || creature.CombatState == null) return;
        if (SecondaryResourceCmd.Get(player, resourceId) == amount) return;

        var key = (player, resourceId);
        var ownsBridge = EnterBridge(key);
        try
        {
            await SecondaryResourceCmd.Set(player, resourceId, amount, source);
        }
        finally
        {
            if (ownsBridge) ExitBridge(key);
        }
    }

    private static bool EnterBridge((Player Player, string ResourceId) key)
    {
        lock (Sync)
            return ActiveBridges.Add(key);
    }

    private static void ExitBridge((Player Player, string ResourceId) key)
    {
        lock (Sync)
            ActiveBridges.Remove(key);
    }

    private static bool IsBridgeActive((Player Player, string ResourceId) key)
    {
        lock (Sync)
            return ActiveBridges.Contains(key);
    }

    private static void ValidateStableId(SecondaryResourceDefinition definition, string expectedId)
    {
        if (!string.Equals(definition.Id, expectedId, StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"RitsuLib genero el recurso '{definition.Id}', se esperaba el ID estable '{expectedId}'.");
    }

    private sealed class LegacyPowerBridge : ISecondaryResourceHookListener
    {
        public decimal ModifyMaxSecondaryResource(SecondaryResourceMaxContext context, decimal amount)
        {
            return string.Equals(context.Definition.Id, NpChargeResourceId, StringComparison.OrdinalIgnoreCase)
                   && context.Player.Creature is { } creature
                ? amount + NpCharge.Max(creature) - NpChargePower.Max
                : amount;
        }

        public async Task AfterSecondaryResourceChanged(SecondaryResourceChangeContext context)
        {
            var resourceId = context.Definition.Id;
            if (!string.Equals(resourceId, NpChargeResourceId, StringComparison.OrdinalIgnoreCase)
                && !string.Equals(resourceId, CritStarsResourceId, StringComparison.OrdinalIgnoreCase))
                return;

            var key = (context.Player, resourceId);
            if (IsBridgeActive(key) || context.Player.Creature is not { } creature) return;

            EnterBridge(key);
            try
            {
                var cardSource = context.Source as CardModel;
                if (string.Equals(resourceId, NpChargeResourceId, StringComparison.OrdinalIgnoreCase))
                {
                    var legacy = NpCharge.Current(creature);
                    if (context.NewAmount > legacy)
                        await NpCharge.Gain(
                            new BlockingPlayerChoiceContext(), creature, context.NewAmount - legacy, cardSource);
                    else if (context.NewAmount < legacy)
                        await NpCharge.Spend(
                            new BlockingPlayerChoiceContext(), creature, legacy - context.NewAmount, cardSource);

                    await SynchronizeNpFromLegacy(creature, context.Source);
                }
                else
                {
                    var legacy = CritStars.Of(creature);
                    if (context.NewAmount != legacy)
                        await CritStars.Gain(
                            new BlockingPlayerChoiceContext(), creature, context.NewAmount - legacy, cardSource);

                    await SynchronizeStarsFromLegacy(creature, context.Source);
                }
            }
            finally
            {
                ExitBridge(key);
            }
        }
    }
}
