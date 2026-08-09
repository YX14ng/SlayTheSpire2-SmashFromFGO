using Steamworks;

const uint AppId = 2868840;
const ulong BaseLibId = 3737335127;
const ulong RitsuLibId = 3747602295;
const ulong FgoCoreId = 3747876334;

var verifyOnly = args.Contains("--verify-only", StringComparer.OrdinalIgnoreCase);
var items = new[]
{
    new WorkshopItem("FGOCore", FgoCoreId, [BaseLibId, RitsuLibId]),
    new WorkshopItem("MashShielder", 3747876464, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("MorganBerserker", 3747876731, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("ArtoriaCaster", 3747876956, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("MordredSaber", 3751610432, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("GilgameshArcher", 3751610575, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("OkitaSaber", 3751610728, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("OberonPretender", 3751610867, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("SiegfriedSaber", 3751611015, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("TiamatBeast", 3751611145, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("KagetoraLancer", 3773261707, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("ShutenDouji", 3774222164, [BaseLibId, RitsuLibId, FgoCoreId]),
    new WorkshopItem("AstolfoRider", 3774222236, [BaseLibId, RitsuLibId, FgoCoreId]),
};

Environment.SetEnvironmentVariable("SteamAppId", AppId.ToString());
Environment.SetEnvironmentVariable("SteamGameId", AppId.ToString());

var initResult = SteamAPI.InitEx(out var initError);
if (initResult != ESteamAPIInitResult.k_ESteamAPIInitResult_OK)
{
    Console.Error.WriteLine($"Steamworks initialization failed ({initResult}): {initError}");
    return 2;
}

try
{
    Console.WriteLine($"Steam account: {SteamFriends.GetPersonaName()}");
    var initialDependencies = await QueryDependencies(items);
    var missing = FindMissing(items, initialDependencies);

    if (verifyOnly)
    {
        PrintStatus(items, initialDependencies);
        return missing.Count == 0 ? 0 : 1;
    }

    if (missing.Count == 0)
    {
        Console.WriteLine("All required Workshop dependencies are already configured.");
    }
    else
    {
        Console.WriteLine($"Adding {missing.Count} missing Workshop dependency link(s)...");
        foreach (var link in missing)
            await AddDependency(link);
    }

    var finalDependencies = await QueryDependencies(items);
    var stillMissing = FindMissing(items, finalDependencies);
    PrintStatus(items, finalDependencies);

    if (stillMissing.Count != 0)
    {
        Console.Error.WriteLine($"Verification failed: {stillMissing.Count} dependency link(s) are still missing.");
        return 1;
    }

    Console.WriteLine($"Verified {items.Sum(item => item.RequiredDependencies.Count)} dependency links across {items.Length} Workshop items.");
    return 0;
}
finally
{
    SteamAPI.Shutdown();
}

static async Task<Dictionary<ulong, HashSet<ulong>>> QueryDependencies(IReadOnlyList<WorkshopItem> items)
{
    var ids = items.Select(item => (PublishedFileId_t)item.Id).ToArray();
    var query = SteamUGC.CreateQueryUGCDetailsRequest(ids, (uint)ids.Length);
    if (query == UGCQueryHandle_t.Invalid)
        throw new InvalidOperationException("Steam returned an invalid UGC query handle.");

    try
    {
        if (!SteamUGC.SetReturnChildren(query, true))
            throw new InvalidOperationException("Steam rejected SetReturnChildren for the UGC query.");
        if (!SteamUGC.SetAllowCachedResponse(query, 0))
            throw new InvalidOperationException("Steam rejected SetAllowCachedResponse for the UGC query.");

        var result = await AwaitCall<SteamUGCQueryCompleted_t>(SteamUGC.SendQueryUGCRequest(query));
        if (result.m_eResult != EResult.k_EResultOK)
            throw new InvalidOperationException($"Steam UGC query failed with {result.m_eResult}.");

        var dependencies = new Dictionary<ulong, HashSet<ulong>>();
        for (uint index = 0; index < result.m_unNumResultsReturned; index++)
        {
            if (!SteamUGC.GetQueryUGCResult(query, index, out var details))
                throw new InvalidOperationException($"Steam did not return UGC result {index}.");

            var parentId = (ulong)details.m_nPublishedFileId;
            var children = new HashSet<ulong>();
            if (details.m_unNumChildren > 0)
            {
                var childIds = new PublishedFileId_t[details.m_unNumChildren];
                if (!SteamUGC.GetQueryUGCChildren(query, index, childIds, details.m_unNumChildren))
                    throw new InvalidOperationException($"Steam did not return dependencies for item {parentId}.");
                foreach (var childId in childIds)
                    children.Add((ulong)childId);
            }

            dependencies[parentId] = children;
        }

        foreach (var item in items)
            dependencies.TryAdd(item.Id, []);
        return dependencies;
    }
    finally
    {
        SteamUGC.ReleaseQueryUGCRequest(query);
    }
}

static List<DependencyLink> FindMissing(
    IEnumerable<WorkshopItem> items,
    IReadOnlyDictionary<ulong, HashSet<ulong>> currentDependencies)
{
    return items
        .SelectMany(item => item.RequiredDependencies
            .Where(dependencyId => !currentDependencies[item.Id].Contains(dependencyId))
            .Select(dependencyId => new DependencyLink(item.Name, item.Id, dependencyId)))
        .ToList();
}

static async Task AddDependency(DependencyLink link)
{
    var result = await AwaitCall<AddUGCDependencyResult_t>(
        SteamUGC.AddDependency((PublishedFileId_t)link.ParentId, (PublishedFileId_t)link.ChildId));
    if (result.m_eResult != EResult.k_EResultOK)
        throw new InvalidOperationException(
            $"Steam rejected {link.ParentName} ({link.ParentId}) -> {link.ChildId}: {result.m_eResult}.");

    Console.WriteLine($"  added {link.ParentName} ({link.ParentId}) -> {link.ChildId}");
}

static async Task<T> AwaitCall<T>(SteamAPICall_t apiCall) where T : struct
{
    if (apiCall == SteamAPICall_t.Invalid)
        throw new InvalidOperationException($"Steam returned an invalid API call for {typeof(T).Name}.");

    var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
    using var callResult = CallResult<T>.Create((result, ioFailure) =>
    {
        if (ioFailure)
            completion.TrySetException(new InvalidOperationException($"Steam I/O failure while awaiting {typeof(T).Name}."));
        else
            completion.TrySetResult(result);
    });
    callResult.Set(apiCall);

    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
    while (!completion.Task.IsCompleted)
    {
        timeout.Token.ThrowIfCancellationRequested();
        SteamAPI.RunCallbacks();
        await Task.Delay(20, timeout.Token);
    }

    return await completion.Task;
}

static void PrintStatus(
    IEnumerable<WorkshopItem> items,
    IReadOnlyDictionary<ulong, HashSet<ulong>> currentDependencies)
{
    foreach (var item in items)
    {
        var missing = item.RequiredDependencies
            .Where(dependencyId => !currentDependencies[item.Id].Contains(dependencyId))
            .ToArray();
        Console.WriteLine($"{item.Name}: {(missing.Length == 0 ? "OK" : $"missing {string.Join(", ", missing)}")}");
    }
}

internal sealed record WorkshopItem(string Name, ulong Id, IReadOnlyList<ulong> RequiredDependencies);
internal sealed record DependencyLink(string ParentName, ulong ParentId, ulong ChildId);
