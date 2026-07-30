using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;

if (args.Length < 4 || args[0] is not ("main" or "beta") || args[1] is not ("main" or "beta"))
{
    Console.Error.WriteLine(
        "Usage: CompatibilityProbe <runtime-branch> <build-branch> <game-assembly-dir> <FGOCore.dll> [dependency-dir ...]");
    return 2;
}

var runtimeBranch = args[0];
var buildBranch = args[1];
var gameAssemblyDir = Path.GetFullPath(args[2]);
var corePath = Path.GetFullPath(args[3]);
var explicitSearchDirectories = args.Skip(4)
    .Select(Path.GetFullPath)
    .Prepend(Path.GetDirectoryName(corePath)!)
    .Prepend(gameAssemblyDir)
    .ToList();

var nugetRoot = Environment.GetEnvironmentVariable("NUGET_PACKAGES")
                ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".nuget", "packages");
var baseLibPackageRoot = Path.Combine(nugetRoot, "alchyr.sts2.baselib");
if (Directory.Exists(baseLibPackageRoot))
{
    explicitSearchDirectories.AddRange(Directory.EnumerateDirectories(baseLibPackageRoot, "*", SearchOption.TopDirectoryOnly)
        .Select(versionDirectory => Path.Combine(versionDirectory, "lib", "net9.0"))
        .Where(directory => File.Exists(Path.Combine(directory, "BaseLib.dll")))
        .OrderByDescending(directory => directory, StringComparer.OrdinalIgnoreCase));
}

var searchDirectories = explicitSearchDirectories.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();

AssemblyLoadContext.Default.Resolving += (_, assemblyName) =>
{
    foreach (var directory in searchDirectories)
    {
        var candidate = Path.Combine(directory, $"{assemblyName.Name}.dll");
        if (File.Exists(candidate)) return AssemblyLoadContext.Default.LoadFromAssemblyPath(candidate);
    }

    return null;
};

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

    var game = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.Combine(gameAssemblyDir, "sts2.dll"));
    var core = AssemblyLoadContext.Default.LoadFromAssemblyPath(corePath);
    var compatibilityType = core.GetType(
        "FGOCore.FGOCoreCode.Compatibility.CreatureCmdCompatibility", throwOnError: true)!;
    RuntimeHelpers.RunClassConstructor(compatibilityType.TypeHandle);

    var supportsCardPlay = (bool)compatibilityType
        .GetProperty("SupportsCardPlayDamageContext", BindingFlags.Static | BindingFlags.Public)!
        .GetValue(null)!;
    Assert(supportsCardPlay == (runtimeBranch == "beta"),
        $"Runtime branch detection mismatch: expected {runtimeBranch}, CardPlay support={supportsCardPlay}.");

    var creature = RequireType(game, "MegaCrit.Sts2.Core.Entities.Creatures.Creature");
    var cardModel = RequireType(game, "MegaCrit.Sts2.Core.Models.CardModel");
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

    Console.WriteLine(
        $"Compatibility OK: build={buildBranch}, runtime={runtimeBranch}, CardPlay={supportsCardPlay}");
    return 0;
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception);
    return 1;
}
