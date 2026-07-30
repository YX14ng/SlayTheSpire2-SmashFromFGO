using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var fix = args.Contains("--fix", StringComparer.OrdinalIgnoreCase);
var rootArg = args.FirstOrDefault(arg => !arg.StartsWith("--", StringComparison.Ordinal));
var root = Path.GetFullPath(rootArg ?? Directory.GetCurrentDirectory());

string[] sourceRoots =
[
    "FGOCore", "MashShielder", "MorganBerserker", "ArtoriaCaster", "MordredSaber",
    "GilgameshArcher", "OkitaSaber", "OberonPretender", "SiegfriedSaber", "Tiamat",
    "KagetoraLancer", "ShutenDouji", "AstolfoRider"
];

var legacyArities = new Dictionary<string, HashSet<int>>(StringComparer.Ordinal)
{
    ["NpCharge.Gain"] = [3],
    ["NpCharge.PayForNpCard"] = [3],
    ["NpCharge.ConsumeAllForNpCard"] = [3],
    ["NpCharge.MarkNpResolvedThisTurn"] = [1],
    ["NpCharge.RefundAfterNpCard"] = [5],
    ["NpCharge.Spend"] = [3],
    ["CritStars.Gain"] = [3],
    ["Stars.Gain"] = [3],
    ["Stars.ConsumeForCrit"] = [3],
    ["Stars.ConsumeExactStars"] = [3],
    ["Aliento.Gain"] = [3],
    ["Aliento.Spend"] = [3, 4],
    ["Aliento.FillToCap"] = [2],
    ["Curses.Apply"] = [4],
    ["Curses.Consume"] = [2],
    ["Lahmu.Spawn"] = [3],
    ["Lahmu.Feed"] = [3],
    ["Lahmu.Devour"] = [2],
    ["TreasurePower.Add"] = [4],
    ["TreasurePower.TrySpend"] = [2],
    ["DebtPower.Add"] = [4],
    ["DebtPower.Forgive"] = [2],
    ["Sleep.TryApply"] = [4],
    ["Sello.Apply"] = [4],
    ["Tos.ShuffleIntoDraw"] = [2],
    ["OberonExtensions.StripPositiveStrengthFromAll"] = [2],
    ["OberonExtensions.SleepAll"] = [4],
    ["NpWindow.ReturnResources"] = [3],
    ["NpWindow.OpenWindow"] = [1, 2, 3]
};

var findings = 0;
var changedFiles = 0;
var utf8NoBom = new UTF8Encoding(false);

foreach (var sourceRoot in sourceRoots)
{
    var directory = Path.Combine(root, sourceRoot);
    if (!Directory.Exists(directory)) continue;

    foreach (var path in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
    {
        if (path.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase)
            || path.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.OrdinalIgnoreCase))
            continue;

        var source = File.ReadAllText(path);
        var tree = CSharpSyntaxTree.ParseText(source, path: path);
        var rootNode = tree.GetRoot();
        var edits = new List<TextEdit>();

        foreach (var method in rootNode.DescendantNodes().OfType<MethodDeclarationSyntax>())
        {
            var contextParameter = method.ParameterList.Parameters.FirstOrDefault(parameter =>
                parameter.Type?.ToString() is "PlayerChoiceContext" or "PlayerChoiceContext?");
            if (contextParameter == null) continue;

            var contextName = contextParameter.Identifier.ValueText;
            var nullableContext = contextParameter.Type?.ToString().EndsWith("?", StringComparison.Ordinal) == true;
            var contextExpression = nullableContext
                ? $"{contextName} ?? new BlockingPlayerChoiceContext()"
                : contextName;
            foreach (var creation in method.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
            {
                var typeName = creation.Type.ToString();
                if (typeName is not ("BlockingPlayerChoiceContext" or "ThrowingPlayerChoiceContext")) continue;
                if (nullableContext && IsFallbackFor(creation, contextName)) continue;

                AddFinding(path, creation, $"creates {typeName} instead of reusing {contextName}");
                edits.Add(new TextEdit(creation.Span.Start, creation.Span.Length, contextExpression));
            }

            foreach (var invocation in method.DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                var key = GetInvocationKey(invocation.Expression);
                if (key == null || !legacyArities.TryGetValue(key, out var arities)) continue;

                var arguments = invocation.ArgumentList.Arguments;
                if (!arities.Contains(arguments.Count)) continue;
                if (arguments.Count > 0 && IsContextArgument(arguments[0].Expression, contextName)) continue;

                AddFinding(path, invocation, $"calls legacy {key} without {contextName}");
                edits.Add(new TextEdit(invocation.ArgumentList.OpenParenToken.Span.End, 0, $"{contextExpression}, "));
            }
        }

        if (!fix || edits.Count == 0) continue;

        foreach (var edit in edits
                     .DistinctBy(edit => (edit.Start, edit.Length, edit.Replacement))
                     .OrderByDescending(edit => edit.Start))
        {
            source = source.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.Replacement);
        }
        File.WriteAllText(path, source, utf8NoBom);
        changedFiles++;
    }
}

Console.WriteLine($"Choice-context audit: {findings} finding(s), {changedFiles} file(s) changed.");
return fix || findings == 0 ? 0 : 1;

void AddFinding(string path, SyntaxNode node, string message)
{
    findings++;
    var line = node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;
    Console.WriteLine($"{Path.GetRelativePath(root, path)}:{line}: {message}");
}

static bool IsContextArgument(ExpressionSyntax expression, string contextName) =>
    expression is IdentifierNameSyntax identifier && identifier.Identifier.ValueText == contextName
    || expression is BinaryExpressionSyntax
    {
        RawKind: (int)SyntaxKind.CoalesceExpression,
        Left: IdentifierNameSyntax coalescedIdentifier
    } && coalescedIdentifier.Identifier.ValueText == contextName;

static bool IsFallbackFor(ObjectCreationExpressionSyntax creation, string contextName) =>
    creation.Parent is BinaryExpressionSyntax
    {
        RawKind: (int)SyntaxKind.CoalesceExpression,
        Left: IdentifierNameSyntax identifier
    } && identifier.Identifier.ValueText == contextName;

static string? GetInvocationKey(ExpressionSyntax expression)
{
    if (expression is GenericNameSyntax generic) return generic.Identifier.ValueText;
    if (expression is not MemberAccessExpressionSyntax member) return null;

    var methodName = member.Name switch
    {
        GenericNameSyntax genericName => genericName.Identifier.ValueText,
        SimpleNameSyntax simpleName => simpleName.Identifier.ValueText,
        _ => member.Name.ToString()
    };
    var qualifier = member.Expression.ToString().Split('.').Last();
    return $"{qualifier}.{methodName}";
}

readonly record struct TextEdit(int Start, int Length, string Replacement);
