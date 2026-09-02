using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

if (args.Length < 4 || args[0] is not ("main" or "beta") || args[1] is not ("main" or "beta"))
{
    Console.Error.WriteLine(
        "Usage: CompatibilityProbe <runtime-branch> <build-branch> <game-assembly-dir> <FGOCore.dll> [artifact.dll ...]");
    return 2;
}

var runtimeBranch = args[0];
var buildBranch = args[1];
var gameAssemblyDir = Path.GetFullPath(args[2]);
var corePath = Path.GetFullPath(args[3]);
var artifactPaths = args.Skip(4).Select(Path.GetFullPath).ToArray();
var explicitSearchDirectories = artifactPaths
    .Select(path => Path.GetDirectoryName(path)!)
    .Prepend(Path.GetDirectoryName(corePath)!)
    .Prepend(gameAssemblyDir)
    .ToList();

string? baseLibRuntimeOverride = null;
var configuredBaseLibRuntime = Environment.GetEnvironmentVariable("FGO_BASELIB_RUNTIME_DLL");
if (!string.IsNullOrWhiteSpace(configuredBaseLibRuntime))
{
    baseLibRuntimeOverride = Path.GetFullPath(configuredBaseLibRuntime);
    if (!File.Exists(baseLibRuntimeOverride))
        throw new FileNotFoundException("Configured BaseLib runtime override was not found.", baseLibRuntimeOverride);
    explicitSearchDirectories.Insert(0, Path.GetDirectoryName(baseLibRuntimeOverride)!);
}

var userProfile = Environment.GetEnvironmentVariable("USERPROFILE")
                  ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
var nugetRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
                ?? Path.Combine(userProfile, ".nuget", "packages");
var baseLibPackageRoot = Path.Combine(nugetRoot, "alchyr.sts2.baselib");
if (Directory.Exists(baseLibPackageRoot))
{
    explicitSearchDirectories.AddRange(Directory.EnumerateDirectories(baseLibPackageRoot, "*", SearchOption.TopDirectoryOnly)
        .Select(versionDirectory => Path.Combine(versionDirectory, "lib", "net9.0"))
        .Where(directory => File.Exists(Path.Combine(directory, "BaseLib.dll")))
        .OrderByDescending(directory => directory, StringComparer.OrdinalIgnoreCase));
}

const string expectedRitsuVersion = "0.5.10";
var ritsuPackageName = runtimeBranch == "main"
    ? "sts2.ritsulib.compat.0.107.1"
    : "sts2.ritsulib";
var ritsuPackageRoot = Path.Combine(nugetRoot, ritsuPackageName, expectedRitsuVersion);
var ritsuAssemblyDirectory = Path.Combine(ritsuPackageRoot, "lib", "net9.0");
if (!File.Exists(Path.Combine(ritsuAssemblyDirectory, "STS2-RitsuLib.dll")))
{
    Console.Error.WriteLine(
        $"RitsuLib {expectedRitsuVersion} for {runtimeBranch} was not found at {ritsuAssemblyDirectory}");
    return 2;
}
explicitSearchDirectories.Insert(0, ritsuAssemblyDirectory);

var searchDirectories = explicitSearchDirectories.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
{
    string? fallback = null;
    foreach (var directory in searchDirectories)
    {
        var candidate = Path.Combine(directory, $"{assemblyName.Name}.dll");
        if (!File.Exists(candidate)) continue;

        fallback ??= candidate;
        if (assemblyName.Version == null ||
            AssemblyName.GetAssemblyName(candidate).Version == assemblyName.Version)
            return AssemblyLoadContext.Default.LoadFromAssemblyPath(candidate);
    }

    return fallback == null ? null : AssemblyLoadContext.Default.LoadFromAssemblyPath(fallback);
};

var baseLibPath = baseLibRuntimeOverride ?? searchDirectories
    .Select(directory => Path.Combine(directory, "BaseLib.dll"))
    .FirstOrDefault(File.Exists);
if (baseLibPath is null)
{
    Console.Error.WriteLine(
        $"BaseLib.dll was not found in the compatibility probe search paths: {string.Join("; ", searchDirectories)}");
    return 2;
}

var baseLib = AssemblyLoadContext.Default.LoadFromAssemblyPath(baseLibPath);
var ritsuLibPath = Path.Combine(ritsuAssemblyDirectory, "STS2-RitsuLib.dll");
var ritsuLib = AssemblyLoadContext.Default.LoadFromAssemblyPath(ritsuLibPath);

try
{
    static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    static Type RequireType(Assembly assembly, string name) =>
        assembly.GetType(name, throwOnError: true)!;

    static MethodInfo RequireDeclaredMethod(Type type, string name, params Type[] parameters) =>
        type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly,
            binder: null, types: parameters, modifiers: null)
        ?? throw new MissingMethodException(type.FullName, $"{name}({string.Join(", ", parameters.Select(t => t.Name))})");

    static MethodInfo RequirePublicStaticMethod(Type type, string name, params Type[] parameters) =>
        type.GetMethod(name, BindingFlags.Static | BindingFlags.Public,
            binder: null, types: parameters, modifiers: null)
        ?? throw new MissingMethodException(type.FullName, $"{name}({string.Join(", ", parameters.Select(t => t.Name))})");

    static string? GetReferenceAssemblyName(MetadataReader metadata, EntityHandle handle)
    {
        while (!handle.IsNil)
        {
            switch (handle.Kind)
            {
                case HandleKind.AssemblyReference:
                    return metadata.GetString(metadata.GetAssemblyReference((AssemblyReferenceHandle)handle).Name);
                case HandleKind.TypeReference:
                    handle = metadata.GetTypeReference((TypeReferenceHandle)handle).ResolutionScope;
                    continue;
                default:
                    return null;
            }
        }

        return null;
    }

    static bool IsRuntimeContractAssembly(string? assemblyName) =>
        assemblyName is not null && assemblyName.ToUpperInvariant() is
            "STS2" or "BASELIB" or "STS2-RITSULIB" or "FGOCORE" or "0HARMONY" or "GODOTSHARP";

    static (int Types, int Members) ValidateRuntimeReferences(
        string artifactPath,
        Module module,
        string runtime)
    {
        using var stream = File.OpenRead(artifactPath);
        using var pe = new PEReader(stream);
        var metadata = pe.GetMetadataReader();
        var resolvedTypes = 0;
        var resolvedMembers = 0;

        foreach (var handle in metadata.TypeReferences)
        {
            var type = metadata.GetTypeReference(handle);
            var assemblyName = GetReferenceAssemblyName(metadata, type.ResolutionScope);
            if (!IsRuntimeContractAssembly(assemblyName)) continue;

            var token = MetadataTokens.GetToken(handle);
            try
            {
                if (module.ResolveType(token) is null)
                {
                    throw new TypeLoadException($"Metadata token 0x{token:X8} did not resolve.");
                }

                resolvedTypes++;
            }
            catch (Exception exception)
            {
                var namespaceName = metadata.GetString(type.Namespace);
                var typeName = metadata.GetString(type.Name);
                throw new InvalidOperationException(
                    $"{Path.GetFileName(artifactPath)} references {assemblyName} type " +
                    $"{namespaceName}.{typeName} that is unavailable on the {runtime} runtime " +
                    $"(token 0x{token:X8}).", exception);
            }
        }

        foreach (var handle in metadata.MemberReferences)
        {
            var member = metadata.GetMemberReference(handle);
            var assemblyName = GetReferenceAssemblyName(metadata, member.Parent);
            if (!IsRuntimeContractAssembly(assemblyName)) continue;

            var token = MetadataTokens.GetToken(handle);
            try
            {
                if (module.ResolveMember(token) is null)
                {
                    throw new MissingMemberException($"Metadata token 0x{token:X8} did not resolve.");
                }

                resolvedMembers++;
            }
            catch (Exception exception)
            {
                var typeName = member.Parent.Kind == HandleKind.TypeReference
                    ? metadata.GetString(metadata.GetTypeReference((TypeReferenceHandle)member.Parent).Name)
                    : member.Parent.Kind.ToString();
                var memberName = metadata.GetString(member.Name);
                throw new InvalidOperationException(
                    $"{Path.GetFileName(artifactPath)} references {assemblyName} member {typeName}.{memberName} " +
                    $"that is unavailable on the {runtime} runtime (token 0x{token:X8}).", exception);
            }
        }

        return (resolvedTypes, resolvedMembers);
    }

    static IEnumerable<CustomAttributeData> HarmonyPatchAttributes(MemberInfo member) =>
        member.CustomAttributes.Where(attribute =>
            attribute.AttributeType.FullName == "HarmonyLib.HarmonyPatch");

    static bool IsHarmonyPatchMethod(MethodInfo method) =>
        method.Name is "Prefix" or "Postfix" or "Transpiler" or "Finalizer" or
            "InnerPrefix" or "InnerPostfix" ||
        method.CustomAttributes.Any(attribute => attribute.AttributeType.FullName is
            "HarmonyLib.HarmonyPrefix" or "HarmonyLib.HarmonyPostfix" or
            "HarmonyLib.HarmonyTranspiler" or "HarmonyLib.HarmonyFinalizer" or
            "HarmonyLib.HarmonyInnerPrefix" or "HarmonyLib.HarmonyInnerPostfix");

    static void MergeHarmonyPatchAttribute(
        CustomAttributeData attribute,
        ref Type? declaringType,
        ref string? methodName,
        ref string? methodType,
        ref Type[]? argumentTypes)
    {
        var parameters = attribute.Constructor.GetParameters();
        for (var index = 0; index < parameters.Length; index++)
        {
            var parameter = parameters[index];
            var argument = attribute.ConstructorArguments[index];
            switch (parameter.Name)
            {
                case "declaringType" when argument.Value is Type type:
                    declaringType = type;
                    break;
                case "methodName" when argument.Value is string name:
                    methodName = name;
                    break;
                case "methodType" when argument.Value is not null:
                    methodType = Enum.GetName(argument.ArgumentType, argument.Value);
                    break;
                case "argumentTypes" when argument.Value is IReadOnlyCollection<CustomAttributeTypedArgument> values:
                    argumentTypes = values.Select(value => (Type)value.Value!).ToArray();
                    break;
            }
        }
    }

    static MethodBase? ResolveHarmonyTarget(
        Type declaringType,
        string? methodName,
        string? methodType,
        Type[]? argumentTypes)
    {
        const BindingFlags declared = BindingFlags.Instance | BindingFlags.Static |
                                      BindingFlags.Public | BindingFlags.NonPublic |
                                      BindingFlags.DeclaredOnly;
        methodType ??= "Normal";

        if (methodType == "Getter")
        {
            return methodName is null
                ? null
                : declaringType.GetProperty(methodName, declared)?.GetGetMethod(nonPublic: true);
        }
        if (methodType == "Setter")
        {
            return methodName is null
                ? null
                : declaringType.GetProperty(methodName, declared)?.GetSetMethod(nonPublic: true);
        }
        if (methodType == "Constructor")
        {
            return argumentTypes is null
                ? declaringType.GetConstructors(declared).SingleOrDefault()
                : declaringType.GetConstructor(declared, binder: null, argumentTypes, modifiers: null);
        }
        if (methodType == "StaticConstructor")
        {
            return declaringType.TypeInitializer;
        }
        if (methodType != "Normal" || string.IsNullOrEmpty(methodName)) return null;

        var methods = declaringType.GetMethods(declared).Where(method => method.Name == methodName);
        if (argumentTypes is not null)
        {
            methods = methods.Where(method => method.GetParameters()
                .Select(parameter => parameter.ParameterType)
                .SequenceEqual(argumentTypes));
        }
        return methods.SingleOrDefault();
    }

    static int ValidateHarmonyTargets(Assembly artifact, string runtime)
    {
        const BindingFlags declared = BindingFlags.Instance | BindingFlags.Static |
                                      BindingFlags.Public | BindingFlags.NonPublic |
                                      BindingFlags.DeclaredOnly;
        var resolved = 0;

        foreach (var type in artifact.GetTypes())
        {
            var classAttributes = HarmonyPatchAttributes(type).ToArray();
            if (classAttributes.Length == 0) continue;

            foreach (var patchMethod in type.GetMethods(declared).Where(IsHarmonyPatchMethod))
            {
                Type? declaringType = null;
                string? methodName = null;
                string? methodType = null;
                Type[]? argumentTypes = null;
                foreach (var attribute in classAttributes.Concat(HarmonyPatchAttributes(patchMethod)))
                {
                    MergeHarmonyPatchAttribute(
                        attribute, ref declaringType, ref methodName, ref methodType, ref argumentTypes);
                }

                MethodBase? target;
                if (declaringType is not null)
                {
                    target = ResolveHarmonyTarget(declaringType, methodName, methodType, argumentTypes);
                }
                else
                {
                    var prepare = type.GetMethod("Prepare", declared, binder: null, Type.EmptyTypes, modifiers: null);
                    if (prepare?.ReturnType == typeof(bool) && !(bool)prepare.Invoke(null, null)!)
                    {
                        continue;
                    }

                    var targetMethod = type.GetMethod(
                        "TargetMethod", declared, binder: null, Type.EmptyTypes, modifiers: null);
                    target = targetMethod?.Invoke(null, null) as MethodBase;
                }

                if (target is null)
                {
                    var signature = argumentTypes is null
                        ? string.Empty
                        : $"({string.Join(", ", argumentTypes.Select(type => type.Name))})";
                    throw new MissingMethodException(
                        $"Harmony target for {type.FullName}.{patchMethod.Name} is unavailable on " +
                        $"{runtime}: {declaringType?.FullName ?? "<dynamic>"}.{methodName}{signature} " +
                        $"[{methodType ?? "Normal"}].");
                }

                resolved++;
            }
        }

        return resolved;
    }

    var game = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(gameAssemblyDir, "sts2.dll"));
    var core = AssemblyLoadContext.Default.LoadFromAssemblyPath(corePath);
    RequireType(core, "FGOCore.FGOCoreCode.Compatibility.PreparedOrobasUpgradeCompatibility");
    RequireType(core, "FGOCore.FGOCoreCode.Compatibility.FgoRelicReplacementStateCompatibility");
    RequireType(core, "FGOCore.FGOCoreCode.Compatibility.SeaGlassCompatibility");
    var customTypeTextCard = RequireType(baseLib, "BaseLib.Abstracts.ICustomTypeTextCard");
    var customCardPoolModel = RequireType(baseLib, "BaseLib.Abstracts.CustomCardPoolModel");
    var transcendenceCard = RequireType(baseLib, "BaseLib.Abstracts.ITranscendenceCard");
    var colorfulPhilosophersPool = RequireType(
        ritsuLib, "STS2RitsuLib.Scaffolding.Characters.IModColorfulPhilosophersCardPool");
    var commandTyped = RequireType(core, "FGOCore.FGOCoreCode.CardTypes.ICommandTyped");
    Assert(customTypeTextCard.IsAssignableFrom(commandTyped),
        "ICommandTyped must expose BaseLib's custom card-type plaque API.");

    var ritsuIntegration = RequireType(core, "FGOCore.FGOCoreCode.Ritsu.FgoRitsuIntegration");
    Assert(ritsuIntegration.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(method =>
            method.Name == "RegisterCharacterMod" && method.IsGenericMethodDefinition &&
            method.GetGenericArguments().Length == 1 &&
            method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual([typeof(string), typeof(string)])),
        "FgoRitsuIntegration must expose the character-owned Yummy Cookie registration overload.");
    Assert(ritsuIntegration.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(method =>
            method.Name == "RegisterCharacterMod" && method.IsGenericMethodDefinition &&
            method.GetGenericArguments().Length == 3 &&
            method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual([typeof(string), typeof(string)])),
        "FgoRitsuIntegration must expose the RitsuLib Orobas registration overload.");
    Assert(ritsuIntegration.GetMethods(BindingFlags.Public | BindingFlags.Static).Any(method =>
            method.Name == "RegisterCharacterMod" && method.IsGenericMethodDefinition &&
            method.GetGenericArguments().Length == 5 &&
            method.GetParameters().Select(parameter => parameter.ParameterType).SequenceEqual([typeof(string), typeof(string)])),
        "FgoRitsuIntegration must expose the combined Orobas/Archaic Tooth registration overload.");
    var ritsuContentRegistry = RequireType(ritsuLib, "STS2RitsuLib.Content.ModContentRegistry");
    var getQualifiedCardTagId = RequirePublicStaticMethod(
        ritsuContentRegistry, "GetQualifiedCardTagId", typeof(string), typeof(string));
    var getQualifiedModelCapabilityId = RequirePublicStaticMethod(
        ritsuContentRegistry, "GetQualifiedModelCapabilityId", typeof(string), typeof(string));
    var expectedRitsuIds = new Dictionary<string, string>
    {
        ["BusterTagId"] = (string)getQualifiedCardTagId.Invoke(null, ["FGOCore", "COMMAND_BUSTER"])!,
        ["ArtsTagId"] = (string)getQualifiedCardTagId.Invoke(null, ["FGOCore", "COMMAND_ARTS"])!,
        ["QuickTagId"] = (string)getQualifiedCardTagId.Invoke(null, ["FGOCore", "COMMAND_QUICK"])!,
        ["CommandTagCapabilityId"] = (string)getQualifiedModelCapabilityId.Invoke(
            null, ["FGOCore", "COMMAND_TAG"])!
    };
    foreach (var (fieldName, generatedId) in expectedRitsuIds)
    {
        var publishedId = (string)(ritsuIntegration.GetField(
            fieldName, BindingFlags.Public | BindingFlags.Static)?.GetRawConstantValue()
            ?? throw new MissingFieldException(ritsuIntegration.FullName, fieldName));
        Assert(string.Equals(publishedId, generatedId, StringComparison.Ordinal),
            $"{fieldName} must match RitsuLib's official ID generator: expected {generatedId}, got {publishedId}.");
    }

    var secondaryResources = RequireType(core, "FGOCore.FGOCoreCode.Ritsu.FgoSecondaryResources");
    var secondaryResourceRegistry = RequireType(
        ritsuLib, "STS2RitsuLib.Combat.SecondaryResources.ModSecondaryResourceRegistry");
    var getSecondaryResourceId = RequirePublicStaticMethod(
        secondaryResourceRegistry, "GetResourceId", typeof(string), typeof(string));
    var expectedSecondaryResourceIds = new Dictionary<string, string>
    {
        ["NpChargeResourceId"] = (string)getSecondaryResourceId.Invoke(null, ["FGOCore", "NP_CHARGE"])!,
        ["CritStarsResourceId"] = (string)getSecondaryResourceId.Invoke(null, ["FGOCore", "CRIT_STARS"])!
    };
    foreach (var (fieldName, generatedId) in expectedSecondaryResourceIds)
    {
        var publishedId = (string)(secondaryResources.GetField(
            fieldName, BindingFlags.Public | BindingFlags.Static)?.GetRawConstantValue()
            ?? throw new MissingFieldException(secondaryResources.FullName, fieldName));
        Assert(string.Equals(publishedId, generatedId, StringComparison.Ordinal),
            $"{fieldName} must match RitsuLib's official secondary-resource ID generator: " +
            $"expected {generatedId}, got {publishedId}.");
    }

    var ritsuCapability = RequireType(core, "FGOCore.FGOCoreCode.Ritsu.FgoCommandTagCapability");
    Assert(ritsuCapability.GetInterfaces().Any(type =>
            type.FullName == "STS2RitsuLib.Models.Capabilities.ICardPropertyContributor"),
        "FgoCommandTagCapability must contribute RitsuLib card properties.");

    var touchOfOrobas = RequireType(game, "MegaCrit.Sts2.Core.Models.Relics.TouchOfOrobas");
    Assert(touchOfOrobas.GetField("_upgradedRelic", BindingFlags.Instance | BindingFlags.NonPublic) is not null,
        "TouchOfOrobas._upgradedRelic reflection target is missing.");
    var compatibilityType = core.GetType(
        "FGOCore.FGOCoreCode.Compatibility.CreatureCmdCompatibility", throwOnError: true)!;
    RuntimeHelpers.RunClassConstructor(compatibilityType.TypeHandle);

    var supportsCardPlay = (bool)compatibilityType
        .GetProperty("SupportsCardPlayDamageContext", BindingFlags.Static | BindingFlags.Public)!
        .GetValue(null)!;
    Assert(supportsCardPlay == (runtimeBranch == "beta"),
        $"Runtime branch detection mismatch: expected {runtimeBranch}, CardPlay support={supportsCardPlay}.");

    // CardCmd.Exhaust cambio su retorno (Task -> Task<CardPileAddResult?>) en BETA 0.111.0 sin tocar
    // los parametros. Correr el ctor estatico fuerza la resolucion del puente: si la sobrecarga no
    // existe en este runtime, RequireExhaust lanza y el probe falla acá en vez de al jugar la carta.
    // No se compara contra runtimeBranch porque la referencia BETA de .compat puede ser anterior a
    // 0.111.0; lo que se exige es que el puente ligue, no contra que rama ligo.
    var cardCmdCompatibility = RequireType(core, "FGOCore.FGOCoreCode.Compatibility.CardCmdCompatibility");
    RuntimeHelpers.RunClassConstructor(cardCmdCompatibility.TypeHandle);
    var supportsExhaustResult = (bool)cardCmdCompatibility
        .GetProperty("SupportsExhaustResult", BindingFlags.Static | BindingFlags.Public)!
        .GetValue(null)!;
    Assert(RequirePublicStaticMethod(cardCmdCompatibility, "Exhaust",
        RequireType(game, "MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext"),
        RequireType(game, "MegaCrit.Sts2.Core.Models.CardModel"),
        typeof(bool), typeof(bool)) is not null,
        "CardCmdCompatibility.Exhaust must expose the runtime's Exhaust parameter list.");

    var creature = RequireType(game, "MegaCrit.Sts2.Core.Entities.Creatures.Creature");
    var cardModel = RequireType(game, "MegaCrit.Sts2.Core.Models.CardModel");
    var relicModel = RequireType(game, "MegaCrit.Sts2.Core.Models.RelicModel");
    var cardPlay = RequireType(game, "MegaCrit.Sts2.Core.Entities.Cards.CardPlay");
    var valueProp = RequireType(game, "MegaCrit.Sts2.Core.ValueProps.ValueProp");
    var choiceContext = RequireType(game, "MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext");
    var abstractModel = RequireType(game, "MegaCrit.Sts2.Core.Models.AbstractModel");
    var power = RequireType(core, "FGOCore.FGOCoreCode.FGOCorePower");
    var hooks = RequireType(core, "FGOCore.FGOCoreCode.Compatibility.IFgoDamageHooks");

    Assert(hooks.GetMethods().Length == 3, "IFgoDamageHooks must expose exactly three damage modifiers.");
    Assert(hooks.GetMethods().All(method => method.GetParameters().Last().ParameterType == cardPlay),
        "Every IFgoDamageHooks method must preserve CardPlay context.");

    var npCharge = RequireType(core, "FGOCore.FGOCoreCode.Np.NpCharge");
    var critStars = RequireType(core, "FGOCore.FGOCoreCode.Stars.CritStars");
    var curses = RequireType(core, "FGOCore.FGOCoreCode.Curses.Curses");
    var lahmu = RequireType(core, "FGOCore.FGOCoreCode.Lahmu.Lahmu");
    var legacyGain = new[] { creature, typeof(int), cardModel };
    var contextualGain = new[] { choiceContext, creature, typeof(int), cardModel };

    Assert(RequirePublicStaticMethod(npCharge, "Gain", legacyGain).ReturnType == typeof(Task),
        "NpCharge legacy Gain signature changed.");
    Assert(RequirePublicStaticMethod(npCharge, "Gain", contextualGain).ReturnType == typeof(Task),
        "NpCharge contextual Gain signature is missing.");
    Assert(RequirePublicStaticMethod(npCharge, "Spend", legacyGain).ReturnType == typeof(Task<bool>),
        "NpCharge legacy Spend signature changed.");
    Assert(RequirePublicStaticMethod(npCharge, "Spend", contextualGain).ReturnType == typeof(Task<bool>),
        "NpCharge contextual Spend signature is missing.");
    Assert(RequirePublicStaticMethod(critStars, "Gain", legacyGain).ReturnType == typeof(Task),
        "CritStars legacy Gain signature changed.");
    Assert(RequirePublicStaticMethod(critStars, "Gain", contextualGain).ReturnType == typeof(Task),
        "CritStars contextual Gain signature is missing.");

    var legacyApply = new[] { creature, typeof(int), creature, cardModel };
    var contextualApply = new[] { choiceContext, creature, typeof(int), creature, cardModel };
    var legacyConsume = new[] { creature, typeof(int) };
    var contextualConsume = new[] { choiceContext, creature, typeof(int) };
    Assert(RequirePublicStaticMethod(curses, "Apply", legacyApply).ReturnType == typeof(Task<int>),
        "Curses legacy Apply signature changed.");
    Assert(RequirePublicStaticMethod(curses, "Apply", contextualApply).ReturnType == typeof(Task<int>),
        "Curses contextual Apply signature is missing.");
    Assert(RequirePublicStaticMethod(curses, "Consume", legacyConsume).ReturnType == typeof(Task<int>),
        "Curses legacy Consume signature changed.");
    Assert(RequirePublicStaticMethod(curses, "Consume", contextualConsume).ReturnType == typeof(Task<int>),
        "Curses contextual Consume signature is missing.");
    Assert(RequirePublicStaticMethod(lahmu, "Spawn", legacyGain).ReturnType == typeof(Task<int>),
        "Lahmu legacy Spawn signature changed.");
    Assert(RequirePublicStaticMethod(lahmu, "Spawn", contextualGain).ReturnType == typeof(Task<int>),
        "Lahmu contextual Spawn signature is missing.");
    Assert(RequirePublicStaticMethod(lahmu, "Feed", legacyGain).ReturnType == typeof(Task),
        "Lahmu legacy Feed signature changed.");
    Assert(RequirePublicStaticMethod(lahmu, "Feed", contextualGain).ReturnType == typeof(Task),
        "Lahmu contextual Feed signature is missing.");
    Assert(RequirePublicStaticMethod(lahmu, "Devour", legacyConsume).ReturnType == typeof(Task<int>),
        "Lahmu legacy Devour signature changed.");
    Assert(RequirePublicStaticMethod(lahmu, "Devour", contextualConsume).ReturnType == typeof(Task<int>),
        "Lahmu contextual Devour signature is missing.");

    var legacyFilledType = typeof(Func<,>).MakeGenericType(creature, typeof(Task));
    var contextualFilledType = typeof(Func<,,>).MakeGenericType(choiceContext, creature, typeof(Task));
    Assert(npCharge.GetEvent("GaugeFilled")?.EventHandlerType == legacyFilledType,
        "NpCharge legacy GaugeFilled event changed.");
    Assert(npCharge.GetEvent("GaugeFilledWithContext")?.EventHandlerType == contextualFilledType,
        "NpCharge contextual GaugeFilled event is missing.");

    var additiveParameters = buildBranch == "beta"
        ? new[] { creature, typeof(decimal), valueProp, creature, cardModel, cardPlay }
        : new[] { creature, typeof(decimal), valueProp, creature, cardModel };
    var multiplicativeParameters = additiveParameters;
    var capParameters = buildBranch == "beta"
        ? new[] { creature, valueProp, creature, cardModel, cardPlay }
        : new[] { creature, valueProp, creature, cardModel };

    foreach (var (name, parameters) in new[]
             {
                 ("ModifyDamageAdditive", additiveParameters),
                 ("ModifyDamageMultiplicative", multiplicativeParameters),
                 ("ModifyDamageCap", capParameters)
             })
    {
        var method = RequireDeclaredMethod(power, name, parameters);
        if (runtimeBranch == buildBranch)
        {
            Assert(method.GetBaseDefinition().DeclaringType == abstractModel,
                $"{name} is not a native override for the {runtimeBranch} runtime.");
        }
    }

    var bridgeType = RequireType(core,
        "FGOCore.FGOCoreCode.Compatibility.LegacyDamageHookCompatibility");
    foreach (var nestedName in new[] { "Additive", "Multiplicative", "Cap" })
    {
        var nested = bridgeType.GetNestedType(nestedName, BindingFlags.NonPublic)!;
        var prepare = nested.GetMethod("Prepare", BindingFlags.Static | BindingFlags.NonPublic)!;
        var bridgeEnabled = (bool)prepare.Invoke(null, null)!;
        var expectedBridge = runtimeBranch == "beta" && buildBranch == "main";
        Assert(bridgeEnabled == expectedBridge,
            $"{nestedName} bridge state mismatch: expected {expectedBridge}, actual {bridgeEnabled}.");
    }

    var auditedArtifacts = artifactPaths.Prepend(corePath).ToArray();
    var starterUpgradeTypes = new Dictionary<string, (string Starter, string Upgrade)>(StringComparer.OrdinalIgnoreCase)
    {
        ["MashShielder"] = (
            "MashShielder.MashShielderCode.Relics.RoundTableFragment",
            "MashShielder.MashShielderCode.Relics.LordCamelotRestored"),
        ["MorganBerserker"] = (
            "MorganBerserker.MorganBerserkerCode.Relics.QueensScepter",
            "MorganBerserker.MorganBerserkerCode.Relics.WorldsEndCoronation"),
        ["ArtoriaCaster"] = (
            "ArtoriaCaster.ArtoriaCasterCode.Relics.SelectionStaff",
            "ArtoriaCaster.ArtoriaCasterCode.Relics.ForgedSacredSword"),
        ["MordredSaber"] = (
            "MordredSaber.MordredSaberCode.Relics.ClarentTheStolenSword",
            "MordredSaber.MordredSaberCode.Relics.ClarentOverloadedWithHatred"),
        ["GilgameshArcher"] = (
            "GilgameshArcher.GilgameshArcherCode.Relics.BabIlu",
            "GilgameshArcher.GilgameshArcherCode.Relics.EaSwordOfRupture"),
        ["OkitaSaber"] = (
            "OkitaSaber.OkitaSaberCode.Relics.HaoriAsagi",
            "OkitaSaber.OkitaSaberCode.Relics.FlowerOfImperialCapital"),
        ["OberonPretender"] = (
            "OberonPretender.OberonPretenderCode.Relics.DreamContract",
            "OberonPretender.OberonPretenderCode.Relics.BookOfDreamsEnd"),
        ["SiegfriedSaber"] = (
            "SiegfriedSaber.SiegfriedSaberCode.Relics.LindenLeaf",
            "SiegfriedSaber.SiegfriedSaberCode.Relics.FafnirHeartblood"),
        ["TiamatBeast"] = (
            "TiamatBeast.TiamatCode.Relics.SeaOfLifeWomb",
            "TiamatBeast.TiamatCode.Relics.SeaOfLifeGenesis"),
        ["KagetoraLancer"] = (
            "KagetoraLancer.KagetoraLancerCode.Relics.JeweledPagodaOfBishamonten",
            "KagetoraLancer.KagetoraLancerCode.Relics.GreatPagodaOfBishamonten"),
        ["ShutenDouji"] = (
            "ShutenDouji.ShutenDoujiCode.Relics.ScarletGourd",
            "ShutenDouji.ShutenDoujiCode.Relics.InexhaustibleGourd"),
        ["AstolfoRider"] = (
            "AstolfoRider.AstolfoRiderCode.Relics.ReasonEvaporatedRelic",
            "AstolfoRider.AstolfoRiderCode.Relics.CompletelyEvaporatedReason")
    };
    var transcendenceTypes = new Dictionary<string, (string Starter, string Transcendence)>(StringComparer.OrdinalIgnoreCase)
    {
        ["MashShielder"] = (
            "MashShielder.MashShielderCode.Cards.Basic.ShieldBash",
            "MashShielder.MashShielderCode.Cards.Rare.PaladinAssault"),
        ["MorganBerserker"] = (
            "MorganBerserker.MorganBerserkerCode.Cards.Basic.LanceOfTheWorldsEnd",
            "MorganBerserker.MorganBerserkerCode.Cards.Rare.FromTheWorldsEnd"),
        ["ArtoriaCaster"] = (
            "ArtoriaCaster.ArtoriaCasterCode.Cards.Basic.SummerOutburst",
            "ArtoriaCaster.ArtoriaCasterCode.Cards.Rare.SummerComet"),
        ["MordredSaber"] = (
            "MordredSaber.MordredSaberCode.Cards.Basic.Rebellion",
            "MordredSaber.MordredSaberCode.Cards.Rare.CoupDEtat"),
        ["GilgameshArcher"] = (
            "GilgameshArcher.GilgameshArcherCode.Cards.Basic.GateOfBabylon",
            "GilgameshArcher.GilgameshArcherCode.Cards.Rare.KingsArsenal"),
        ["OkitaSaber"] = (
            "OkitaSaber.OkitaSaberCode.Cards.Basic.Shukuchi",
            "OkitaSaber.OkitaSaberCode.Cards.Rare.InfiniteInstant"),
        ["OberonPretender"] = (
            "OberonPretender.OberonPretenderCode.Cards.Basic.Nightfall",
            "OberonPretender.OberonPretenderCode.Cards.Rare.EndOfTheTale"),
        ["SiegfriedSaber"] = (
            "SiegfriedSaber.SiegfriedSaberCode.Cards.Basic.BloodBaptism",
            "SiegfriedSaber.SiegfriedSaberCode.Cards.Rare.DragonbloodAscendant"),
        ["TiamatBeast"] = (
            "TiamatBeast.TiamatCode.Cards.Basic.SpawnLahmu",
            "TiamatBeast.TiamatCode.Cards.Rare.ElevenBelLahmu"),
        ["KagetoraLancer"] = (
            "KagetoraLancer.KagetoraLancerCode.Cards.Basic.IncarnationOfBishamonten",
            "KagetoraLancer.KagetoraLancerCode.Cards.Rare.ManifestationOfBishamonten"),
        ["ShutenDouji"] = (
            "ShutenDouji.ShutenDoujiCode.Cards.Basic.FruityWineAroma",
            "ShutenDouji.ShutenDoujiCode.Cards.Rare.FruityAromaEx"),
        ["AstolfoRider"] = (
            "AstolfoRider.AstolfoRiderCode.Cards.Basic.PaladinsHunch",
            "AstolfoRider.AstolfoRiderCode.Cards.Rare.PerfectImprovisation")
    };
    var resolvedRuntimeTypes = 0;
    var resolvedRuntimeMembers = 0;
    var resolvedHarmonyTargets = 0;
    foreach (var artifactPath in auditedArtifacts)
    {
        if (!File.Exists(artifactPath))
        {
            throw new FileNotFoundException("Compatibility artifact not found.", artifactPath);
        }

        var artifact = string.Equals(artifactPath, corePath, StringComparison.OrdinalIgnoreCase)
            ? core
            : AssemblyLoadContext.Default.LoadFromAssemblyPath(artifactPath);
        Assert(artifact.GetReferencedAssemblies().Any(reference =>
                string.Equals(reference.Name, "STS2-RitsuLib", StringComparison.OrdinalIgnoreCase)),
            $"{Path.GetFileName(artifactPath)} must directly require RitsuLib.");

        if (starterUpgradeTypes.TryGetValue(artifact.GetName().Name ?? string.Empty, out var upgradeTypes))
        {
            var characterPools = artifact.GetTypes()
                .Where(type => !type.IsAbstract && customCardPoolModel.IsAssignableFrom(type))
                .ToArray();
            Assert(characterPools.Length == 1,
                $"{artifact.GetName().Name} must expose exactly one custom character card pool; " +
                $"found {characterPools.Length}.");
            Assert(colorfulPhilosophersPool.IsAssignableFrom(characterPools[0]),
                $"{characterPools[0].FullName} must opt into RitsuLib's Colorful Philosophers integration.");

            var starter = RequireType(artifact, upgradeTypes.Starter);
            var upgrade = RequireType(artifact, upgradeTypes.Upgrade);
            var mapping = starter.GetMethod(
                "GetUpgradeReplacement",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                ?? throw new MissingMethodException(starter.FullName, "GetUpgradeReplacement()");
            Assert(mapping.ReturnType.FullName == "MegaCrit.Sts2.Core.Models.RelicModel",
                $"{starter.FullName} has an invalid Orobas replacement signature.");
            Assert(relicModel.IsAssignableFrom(upgrade),
                $"{upgrade.FullName} is not a valid relic model type.");

            if (string.Equals(artifact.GetName().Name, "SiegfriedSaber", StringComparison.OrdinalIgnoreCase))
            {
                var startingScales = starter.GetProperty(
                    "StartingScales",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                    ?? throw new MissingMemberException(starter.FullName, "StartingScales");
                Assert(startingScales.PropertyType == typeof(int) && startingScales.CanRead && startingScales.CanWrite,
                    $"{starter.FullName}.StartingScales must be a mutable Int32 saved property.");
                Assert(startingScales.CustomAttributes.Any(attribute =>
                        attribute.AttributeType.Name == "SavedPropertyAttribute"),
                    $"{starter.FullName}.StartingScales must carry SavedPropertyAttribute.");
                Assert(starter.IsAssignableFrom(upgrade),
                    $"{upgrade.FullName} must inherit {starter.FullName} so Orobas can preserve starting Scales.");
            }
        }

        if (transcendenceTypes.TryGetValue(artifact.GetName().Name ?? string.Empty, out var cardTypes))
        {
            var starter = RequireType(artifact, cardTypes.Starter);
            var transcendence = RequireType(artifact, cardTypes.Transcendence);
            Assert(transcendenceCard.IsAssignableFrom(starter),
                $"{starter.FullName} must implement BaseLib ITranscendenceCard.");
            var mapping = starter.GetMethod(
                "GetTranscendenceTransformedCard",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                ?? throw new MissingMethodException(starter.FullName, "GetTranscendenceTransformedCard()");
            Assert(mapping.ReturnType == cardModel,
                $"{starter.FullName} has an invalid Archaic Tooth mapping signature.");
            Assert(cardModel.IsAssignableFrom(transcendence),
                $"{transcendence.FullName} is not a valid card model type.");
        }

        var resolvedReferences = ValidateRuntimeReferences(
            artifactPath, artifact.ManifestModule, runtimeBranch);
        resolvedRuntimeTypes += resolvedReferences.Types;
        resolvedRuntimeMembers += resolvedReferences.Members;
        resolvedHarmonyTargets += ValidateHarmonyTargets(artifact, runtimeBranch);
    }

    Console.WriteLine(
        $"Compatibility OK: build={buildBranch}, runtime={runtimeBranch}, CardPlay={supportsCardPlay}, " +
        $"ExhaustResult={supportsExhaustResult}, " +
        $"artifacts={auditedArtifacts.Length}, runtime types={resolvedRuntimeTypes}, " +
        $"runtime members={resolvedRuntimeMembers}, Harmony targets={resolvedHarmonyTargets}, " +
        $"BaseLib={baseLib.GetName().Version}, RitsuLib={ritsuLib.GetName().Version}, " +
        $"direct RitsuLib references={auditedArtifacts.Length}");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}
