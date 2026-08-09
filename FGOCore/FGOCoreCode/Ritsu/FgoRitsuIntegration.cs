using FGOCore.FGOCoreCode.CardTypes;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Relics;
using STS2RitsuLib;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Models.Capabilities;
using STS2RitsuLib.Scaffolding.Content;

namespace FGOCore.FGOCoreCode.Ritsu;

/// <summary>
/// Integracion compartida con RitsuLib. Publica los tipos de comando FGO como etiquetas dinamicas
/// para que otros mods puedan consultarlos sin depender de las clases concretas de cada Servant.
/// </summary>
public static class FgoRitsuIntegration
{
    public const string BusterTagId = "FGO_CORE_CARDTAG_COMMAND_BUSTER";
    public const string ArtsTagId = "FGO_CORE_CARDTAG_COMMAND_ARTS";
    public const string QuickTagId = "FGO_CORE_CARDTAG_COMMAND_QUICK";
    public const string CommandTagCapabilityId = "FGO_CORE_MODELCAPABILITY_COMMAND_TAG";

    private static readonly object Sync = new();
    private static readonly HashSet<string> RegisteredCharacterMods = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<Type, Type> OrobasUpgradeMappings = [];

    private static ModCardTagDefinition? _busterTag;
    private static ModCardTagDefinition? _artsTag;
    private static ModCardTagDefinition? _quickTag;
    private static bool _initialized;

    /// <summary>Registra las superficies compartidas de FGOCore antes de que ModelDb se congele.</summary>
    public static void Initialize()
    {
        lock (Sync)
        {
            if (_initialized) return;

            var tags = ModCardTagRegistry.For(MainFile.ModId);
            _busterTag = tags.RegisterOwned("COMMAND_BUSTER");
            _artsTag = tags.RegisterOwned("COMMAND_ARTS");
            _quickTag = tags.RegisterOwned("COMMAND_QUICK");

            FgoSecondaryResources.Initialize();

            ValidateStableId(_busterTag, BusterTagId);
            ValidateStableId(_artsTag, ArtsTagId);
            ValidateStableId(_quickTag, QuickTagId);

            var content = RitsuLibFramework.GetContentRegistry(MainFile.ModId);
            content.RegisterModelCapability<FgoCommandTagCapability>(
                ModelPublicEntryOptions.FromStem("COMMAND_TAG"));
            content.ConfigureDefaultModelCapabilities<CardModel>(
                "fgo-command-tags",
                static (card, capabilities) =>
                {
                    if (card is ICommandTyped)
                        capabilities.Add<FgoCommandTagCapability>();
                });

            _initialized = true;
            RitsuLibFramework.SubscribeLifecycleOnce<ModelRegistryInitializedEvent>(
                static _ => AuditRegisteredCommandCards());

            MainFile.Logger.Info(
                $"RitsuLib integrado: tags {BusterTagId}, {ArtsTagId}, {QuickTagId}.");
        }
    }

    /// <summary>Registra un mod de personaje para incluirlo en el diagnostico transversal.</summary>
    public static void RegisterCharacterMod(string modId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modId);
        Initialize();
        lock (Sync)
            RegisteredCharacterMods.Add(modId);
    }

    /// <summary>
    /// Registra un personaje y reemplaza la visual Ironclad que Yummy Cookie usa como fallback
    /// para personajes custom. El perfil reutiliza una reliquia identitaria ya empaquetada por el
    /// mod, por lo que las tres rutas existen tanto en MAIN como en BETA.
    /// </summary>
    public static void RegisterCharacterMod<TCharacter>(string modId, string identityRelicStem)
        where TCharacter : CharacterModel
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identityRelicStem);
        RegisterCharacterMod(modId);

        var relicRoot = $"res://{modId}/images/relics";
        var profile = new RelicAssetProfile(
            $"{relicRoot}/{identityRelicStem}.png",
            $"{relicRoot}/{identityRelicStem}_outline.png",
            $"{relicRoot}/big/{identityRelicStem}.png");

        RitsuLibFramework.GetContentRegistry(modId)
            .RegisterCharacterOwnedRelicVisualOverride<TCharacter, YummyCookie>(profile);

        FgoSecondaryResources.RegisterCharacter<TCharacter>();
    }

    /// <summary>
    /// Registra tambien la mejora de la starter en el contrato oficial de Touch of Orobas. El
    /// overload anterior se conserva para consumidores ya compilados de FGOCore.
    /// </summary>
    public static void RegisterCharacterMod<TCharacter, TStarterRelic, TUpgradedRelic>(
        string modId,
        string identityRelicStem)
        where TCharacter : CharacterModel
        where TStarterRelic : RelicModel
        where TUpgradedRelic : RelicModel
    {
        RegisterCharacterMod<TCharacter>(modId, identityRelicStem);
        RitsuLibFramework.RegisterTouchOfOrobasRefinementMapping<TStarterRelic, TUpgradedRelic>(modId);
        lock (Sync)
            OrobasUpgradeMappings[typeof(TStarterRelic)] = typeof(TUpgradedRelic);
    }

    /// <summary>
    /// Registra en una sola operacion los dos contratos Ancient del personaje: la starter que
    /// refina Orobas y la carta inicial que trasciende Archaic Tooth.
    /// </summary>
    public static void RegisterCharacterMod<
        TCharacter,
        TStarterRelic,
        TUpgradedRelic,
        TStarterCard,
        TAncientCard>(string modId, string identityRelicStem)
        where TCharacter : CharacterModel
        where TStarterRelic : RelicModel
        where TUpgradedRelic : RelicModel
        where TStarterCard : CardModel
        where TAncientCard : CardModel
    {
        RegisterCharacterMod<TCharacter, TStarterRelic, TUpgradedRelic>(modId, identityRelicStem);
        RitsuLibFramework.RegisterArchaicToothTranscendenceMapping<TStarterCard, TAncientCard>(modId);
    }

    internal static bool TryGetOrobasUpgrade(Type starterRelicType, out Type upgradedRelicType)
    {
        lock (Sync)
            return OrobasUpgradeMappings.TryGetValue(starterRelicType, out upgradedRelicType!);
    }

    /// <summary>Obtiene la etiqueta dinamica de RitsuLib correspondiente al comando.</summary>
    public static CardTag GetCommandTag(CommandType commandType)
    {
        EnsureInitialized();
        return commandType switch
        {
            CommandType.Buster => _busterTag!.CardTagValue,
            CommandType.Arts => _artsTag!.CardTagValue,
            CommandType.Quick => _quickTag!.CardTagValue,
            _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null)
        };
    }

    /// <summary>Indica si una carta publica un tipo de comando concreto mediante RitsuLib.</summary>
    public static bool HasCommandTag(CardModel card, CommandType commandType)
    {
        ArgumentNullException.ThrowIfNull(card);
        return card.Tags.Contains(GetCommandTag(commandType));
    }

    internal static IEnumerable<CardTag> GetTagsFor(CardModel card)
    {
        if (card is not ICommandTyped typed) return [];
        return [GetCommandTag(typed.CommandType)];
    }

    private static void AuditRegisteredCommandCards()
    {
        var commandCards = ModelDb.AllCards.OfType<ICommandTyped>()
            .Select(static typed => (Card: (CardModel)typed, Typed: typed))
            .ToArray();

        var invalid = commandCards
            .Where(static entry => !HasExactlyOneMatchingTag(entry.Card, entry.Typed.CommandType))
            .Select(static entry => entry.Card.Id.Entry)
            .OrderBy(static id => id, StringComparer.Ordinal)
            .ToArray();

        string[] registeredMods;
        lock (Sync)
            registeredMods = RegisteredCharacterMods.OrderBy(static id => id, StringComparer.Ordinal).ToArray();

        if (invalid.Length > 0)
        {
            MainFile.Logger.ErrorNoTrace(
                $"RitsuLib audit: {invalid.Length} carta(s) FGO sin un unico tag de comando correcto: " +
                string.Join(", ", invalid));
            return;
        }

        MainFile.Logger.Info(
            $"RitsuLib audit OK: {commandCards.Length} carta(s) de comando; " +
            $"mods activos: {string.Join(", ", registeredMods)}.");
    }

    private static bool HasExactlyOneMatchingTag(CardModel card, CommandType expected)
    {
        var tags = card.Tags;
        var buster = tags.Contains(_busterTag!.CardTagValue);
        var arts = tags.Contains(_artsTag!.CardTagValue);
        var quick = tags.Contains(_quickTag!.CardTagValue);
        var count = (buster ? 1 : 0) + (arts ? 1 : 0) + (quick ? 1 : 0);
        return count == 1 && expected switch
        {
            CommandType.Buster => buster,
            CommandType.Arts => arts,
            CommandType.Quick => quick,
            _ => false
        };
    }

    private static void ValidateStableId(ModCardTagDefinition definition, string expectedId)
    {
        if (!string.Equals(definition.Id, expectedId, StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"RitsuLib genero el tag '{definition.Id}', se esperaba el ID estable '{expectedId}'.");
    }

    private static void EnsureInitialized()
    {
        if (!_initialized)
            throw new InvalidOperationException("FgoRitsuIntegration.Initialize debe ejecutarse antes de consultar tags.");
    }
}

/// <summary>
/// Capacidad sin estado que aporta al modelo la etiqueta de comando actual. RitsuLib se encarga de
/// propagarla a clones y transformaciones de carta.
/// </summary>
public sealed class FgoCommandTagCapability : ModelCapability<CardModel>, ICardPropertyContributor
{
    public IEnumerable<CardTag> GetTags(CardModel card) => FgoRitsuIntegration.GetTagsFor(card);
}
