using Godot;
using FGOCore.FGOCoreCode.Visuals;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// Swaps a creature's combat sprite when it changes form. Each form declares its own
/// SpriteFrames resource via <see cref="FormPower.FramesPath"/> (same animation names),
/// so BaseLib's animation routing keeps working untouched.
/// Frames resources are heavy (hundreds of MB of textures per character). Each character
/// mod registers its OWN forms once (<see cref="RegisterFrames"/>, one call = one group);
/// when a creature enters combat we background-load only its current form. Alternate forms are
/// requested on demand and keep the previous sprite visible while they finish loading.
/// <para>VRAM: do NOT preload every registered group. With N FGO character mods installed,
/// preloading all groups would pin N×(hundreds of frames) into VRAM at once even if you
/// only play one of them — that exhausted VRAM on normal GPUs (cards/intent failed to
/// render → "just health bars", or a hard crash with 3 characters). We scope the preload
/// to the form of the character actually visible.</para>
/// </summary>
public static class FormVisuals
{
    private static readonly HashSet<string> RegisteredPaths = [];
    private static readonly Dictionary<string, float> SpriteXOf = [];
    private static readonly Dictionary<string, float> SpriteYOf = [];
    private static readonly Dictionary<string, float> SpriteScaleOf = [];
    private static readonly Dictionary<string, float> HighQualityScaleOf = [];
    private static readonly Dictionary<string, SpriteFrames> Cache = [];
    private static readonly HashSet<string> Requested = [];
    // Paths whose background load failed once: never re-requested (a broken FramesPath would
    // otherwise re-request every frame → hundreds of "load failed" log lines per form switch).
    private static readonly HashSet<string> Failed = [];

    // Per-sprite generation stamp (Godot meta): a later Apply supersedes earlier ones so a slow
    // load can't overwrite the sprite with a stale form. Meta lives/dies with the node (no leak).
    private const string GenMeta = "fgo_form_gen";
    private const string BaseScaleMeta = "fgo_form_base_scale";

    /// <summary>Register one character's form frame resources (call once at mod init).
    /// Registration identifies paths eligible for adaptive quality; only the visible path loads.</summary>
    public static void RegisterFrames(params string[] paths)
    {
        foreach (var path in paths.Where(p => !string.IsNullOrEmpty(p))) RegisteredPaths.Add(path);
    }

    /// <summary>
    /// Registers a form group and the horizontal sprite pivot required by each frame set.
    /// FGO renders use asymmetric transparent canvases; because player sprites are mirrored,
    /// the compensation must also change when a form swaps to a differently framed model.
    /// </summary>
    public static void RegisterFramesWithSpriteX(params (string Path, float SpriteX)[] forms)
    {
        var valid = forms.Where(f => !string.IsNullOrEmpty(f.Path)).ToArray();
        foreach (var form in valid)
        {
            RegisteredPaths.Add(form.Path);
            SpriteXOf[form.Path] = form.SpriteX;
        }
    }

    /// <summary>
    /// Registers a form group and its full sprite pivot. The vertical component is required when
    /// imported textures use <c>process/size_limit</c>: Godot reduces the canvas coordinates but
    /// does not reduce the scene's <see cref="Node2D.Position"/> automatically.
    /// </summary>
    public static void RegisterFramesWithSpritePosition(
        params (string Path, float SpriteX, float SpriteY)[] forms)
    {
        var valid = forms.Where(f => !string.IsNullOrEmpty(f.Path)).ToArray();
        foreach (var form in valid)
        {
            RegisteredPaths.Add(form.Path);
            SpriteXOf[form.Path] = form.SpriteX;
            SpriteYOf[form.Path] = form.SpriteY;
        }
    }

    /// <summary>
    /// Registers a form group with a complete sprite transform. A separate uniform scale is needed
    /// when forms use differently cropped canvases; applying the same scene scale would move their
    /// heads even when their feet share the same ground line.
    /// </summary>
    public static void RegisterFramesWithSpriteTransform(
        params (string Path, float SpriteX, float SpriteY, float UniformScale)[] forms)
    {
        var valid = forms.Where(f => !string.IsNullOrEmpty(f.Path)).ToArray();
        foreach (var form in valid)
        {
            RegisteredPaths.Add(form.Path);
            SpriteXOf[form.Path] = form.SpriteX;
            SpriteYOf[form.Path] = form.SpriteY;
            SpriteScaleOf[form.Path] = form.UniformScale;
        }
    }

    /// <summary>
    /// Registers a form group whose high-quality imports do not reach the standard 1024 px cap.
    /// The explicit multiplier preserves the exact on-screen size when the source canvas is
    /// smaller than 1024 px (for example, a native 867 px render).
    /// </summary>
    public static void RegisterFramesWithSpriteTransform(
        params (string Path, float SpriteX, float SpriteY, float UniformScale,
            float HighQualityScaleMultiplier)[] forms)
    {
        var valid = forms.Where(f => !string.IsNullOrEmpty(f.Path)).ToArray();
        foreach (var form in valid)
        {
            RegisteredPaths.Add(form.Path);
            SpriteXOf[form.Path] = form.SpriteX;
            SpriteYOf[form.Path] = form.SpriteY;
            SpriteScaleOf[form.Path] = form.UniformScale;
            HighQualityScaleOf[form.Path] = form.HighQualityScaleMultiplier;
        }
    }

    private static FrameResourceSelection Resolve(string logicalPath, FrameQualityTier tier)
    {
        return HighQualityScaleOf.TryGetValue(logicalPath, out var highQualityScale)
            ? FgoVisualQuality.ResolveFrames(logicalPath, tier, highQualityScale)
            : FgoVisualQuality.ResolveFrames(logicalPath, tier);
    }

    private static void PreloadGroup(IEnumerable<FrameResourceSelection> selections)
    {
        foreach (var selection in selections)
        {
            var p = selection.ResourcePath;
            if (Cache.ContainsKey(p) || Requested.Contains(p) || Failed.Contains(p)) continue;
            if (ResourceLoader.LoadThreadedRequest(p, "SpriteFrames", useSubThreads: true) == Error.Ok)
            {
                Requested.Add(p);
            }
        }
    }

    [System.Obsolete("Preloading is now scoped per-character inside Apply; this is a no-op kept for binary compatibility.")]
    public static void PreloadAll() { }

    /// <summary>
    /// Returns the frames ONLY if the background thread has already finished loading them; never
    /// blocks. A synchronous <c>ResourceLoader.Load</c> of these multi-hundred-MB <c>.tres</c> here
    /// would FREEZE the simulation thread — fine as a single-player hitch, but in multiplayer it
    /// stalls the network heartbeat → timeout/disconnect (the "Ortinax form crashes in MP" report,
    /// because Ortinax is the form entered MID-combat that materializes a different resource).
    /// </summary>
    private static SpriteFrames? GetFrames(string path)
    {
        if (Cache.TryGetValue(path, out var cached)) return cached;
        if (Failed.Contains(path)) return null; // known-broken path: fail silently & cheaply, no re-request.

        if (!Requested.Contains(path))
        {
            // Defensive: Apply already kicks the load via PreloadGroup, but if we got here cold,
            // request it in the background and report "not ready yet" without blocking.
            if (ResourceLoader.LoadThreadedRequest(path, "SpriteFrames", useSubThreads: true) == Error.Ok)
            {
                Requested.Add(path);
            }
            else
            {
                // Request rechazado (path inválido/recurso inexistente): marcar Failed — si no, cada
                // poll del ApplyWhenReady re-intentaba el request completo frame a frame.
                Failed.Add(path);
                MainFile.Logger.Error($"FormVisuals: LoadThreadedRequest rejected for {path} (won't retry)");
            }
            return null;
        }

        var status = ResourceLoader.LoadThreadedGetStatus(path);
        if (status != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            if (status is ResourceLoader.ThreadLoadStatus.Failed or ResourceLoader.ThreadLoadStatus.InvalidResource)
            {
                // Cache the failure: log ONCE and stop re-requesting it forever after.
                Requested.Remove(path);
                Failed.Add(path);
                MainFile.Logger.Error($"FormVisuals: background load failed for {path} (won't retry)");
            }
            return null; // still loading → caller retries next frame, never blocks the sim thread.
        }

        var frames = ResourceLoader.LoadThreadedGet(path) as SpriteFrames;
        Requested.Remove(path);
        if (frames != null)
        {
            Cache[path] = frames;
        }
        else
        {
            // Cargó pero no es un SpriteFrames (recurso mal tipado): fallo DEFINITIVO — sin esto,
            // el próximo GetFrames re-encolaba la carga completa del recurso pesado en cada frame.
            Failed.Add(path);
            MainFile.Logger.Error($"FormVisuals: {path} loaded but is not a SpriteFrames (won't retry)");
        }
        return frames;
    }

    // --- Evicción de VRAM por combate (audit 2026-07-04) ---
    // El Cache pineaba los frames DE POR VIDA: entre runs/personajes distintos se acumulaban cientos
    // de MB de VRAM por grupo. Al detectar un combate NUEVO (otro NCombatRoom), los grupos activos del
    // combate anterior pasan a "prev" y se evicta todo lo que no esté en activo∪prev — la ventana de
    // dos combates evita evictar al compañero de co-op que aún no hizo su primer Apply del combate
    // nuevo. El mismo personaje run adelante nunca se evicta (siempre está en el set activo).
    private static ulong _activeCombatId;
    private static HashSet<string> _activePaths = [];
    private static HashSet<string> _prevPaths = [];

    private static void TrackCombatAndEvict(IEnumerable<FrameResourceSelection> selections)
    {
        var room = NCombatRoom.Instance;
        if (room == null) return;
        var id = room.GetInstanceId();
        if (id != _activeCombatId)
        {
            _activeCombatId = id;
            _prevPaths = _activePaths;
            _activePaths = [];
        }
        foreach (var selection in selections) _activePaths.Add(selection.ResourcePath);

        List<string>? evict = null;
        foreach (var key in Cache.Keys)
        {
            if (!_activePaths.Contains(key) && !_prevPaths.Contains(key)) (evict ??= []).Add(key);
        }
        if (evict == null) return;
        foreach (var key in evict)
        {
            Cache.Remove(key);
            Requested.Remove(key);
        }
        MainFile.Logger.Info($"FormVisuals: evicted {evict.Count} cached frame set(s) from inactive character groups");
    }

    /// <summary>
    /// Drops FGOCore's strong references once the combat room leaves the scene tree. The sprites
    /// keep their resources alive until Godot frees the room, but event/rest/merchant rooms no
    /// longer inherit every frame set visited during the previous fight.
    /// </summary>
    internal static void ReleaseCombatFrames(NCombatRoom exitingRoom)
    {
        var exitingCombatId = exitingRoom.GetInstanceId();
        if (_activeCombatId != 0 && _activeCombatId != exitingCombatId) return;

        var released = Cache.Count;
        Cache.Clear();
        _activeCombatId = 0;
        _activePaths = [];
        _prevPaths = [];
        FgoVisualQuality.ReleaseCombatSelection(exitingCombatId);

        if (released > 0)
        {
            MainFile.Logger.Info($"FormVisuals: released {released} cached frame set(s) after combat");
        }
    }

    public static void Apply(Creature creature, FormPower form)
    {
        if (form.FramesPath == null) return;

        Apply(creature, form.FramesPath);
    }

    private static void Apply(Creature creature, string logicalPath)
    {
        // Load only what is visible. Some HD groups contain hundreds of 1024 px frames; warming all
        // alternates at room entry can reserve several GiB before the first turn. A later form switch
        // remains asynchronous and keeps the old sprite visible until the requested form is ready.
        var isMultiplayer = creature.Player?.RunState.Players.Count > 1;
        var tier = FgoVisualQuality.GetFrameTier(isMultiplayer);
        var current = Resolve(logicalPath, tier);
        FrameResourceSelection[] visible = [current];
        TrackCombatAndEvict(visible);
        PreloadGroup(visible);

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite) return;

        // Stamp a new generation: a later Apply on this sprite supersedes this one, so an earlier
        // slow load can't resolve last and overwrite the sprite with the previous form (race).
        var gen = (sprite.HasMeta(GenMeta) ? sprite.GetMeta(GenMeta).AsInt32() : 0) + 1;
        sprite.SetMeta(GenMeta, gen);
        _ = ApplyWhenReadyAsync(sprite, current, gen);
    }

    /// <summary>
    /// Applies quality selection after ally nodes exist. Form characters prefer their saved active
    /// form; single-form characters are identified from the SpriteFrames path embedded in their
    /// scene. This keeps every FGO model on the same asynchronous quality path without requiring a
    /// gameplay power or starter relic solely for presentation.
    /// </summary>
    internal static void ApplyRegisteredInitialFrames(NCombatRoom room)
    {
        foreach (var node in room.CreatureNodes)
        {
            var creature = node.Entity;
            if (creature.Player is null) continue;

            var logicalPath = creature.GetPowerInstances<FormPower>()
                .FirstOrDefault(form => form.FramesPath is not null)?.FramesPath;
            if (logicalPath is null
                && node.FindChild("Sprite", recursive: true, owned: false) is AnimatedSprite2D sprite)
            {
                logicalPath = sprite.SpriteFrames?.ResourcePath;
            }

            if (logicalPath is not null && RegisteredPaths.Contains(logicalPath))
            {
                Apply(creature, logicalPath);
            }
        }
    }

    /// <summary>
    /// Swaps the sprite as soon as the background load finishes, polling the scene's process_frame
    /// signal instead of ever blocking. The current form usually resolves on the first poll; the
    /// loop keeps a later on-demand form switch non-blocking until its frames are ready.
    /// </summary>
    private static async Task ApplyWhenReadyAsync(
        AnimatedSprite2D sprite,
        FrameResourceSelection selection,
        int gen)
    {
        var path = selection.ResourcePath;
        try
        {
            var frames = GetFrames(path);
            if (frames == null)
            {
                if (Failed.Contains(path)) return;
                var tree = sprite.GetTree();
                // Bound the wait (~20s @60fps) so a never-finishing load can't spin forever.
                for (var i = 0; i < 1200 && frames == null; i++)
                {
                    if (tree == null || !GodotObject.IsInstanceValid(sprite)) return;
                    if (Failed.Contains(path)) return; // load failed → stop waiting.
                    if (sprite.GetMeta(GenMeta, 0).AsInt32() != gen) return; // superseded by a newer Apply.
                    await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                    frames = GetFrames(path);
                }
            }

            if (frames == null || !GodotObject.IsInstanceValid(sprite)) return;
            if (sprite.GetMeta(GenMeta, 0).AsInt32() != gen) return; // a newer Apply won; don't stomp it.
            var position = sprite.Position;
            var hasPositionOverride = false;
            if (SpriteXOf.TryGetValue(selection.LogicalPath, out var spriteX))
            {
                position.X = spriteX;
                hasPositionOverride = true;
            }
            if (SpriteYOf.TryGetValue(selection.LogicalPath, out var spriteY))
            {
                position.Y = spriteY;
                hasPositionOverride = true;
            }
            if (hasPositionOverride) sprite.Position = position;
            if (!sprite.HasMeta(BaseScaleMeta)) sprite.SetMeta(BaseScaleMeta, sprite.Scale);
            var baseScale = sprite.GetMeta(BaseScaleMeta).AsVector2();
            if (SpriteScaleOf.TryGetValue(selection.LogicalPath, out var uniformScale))
            {
                baseScale = new Vector2(uniformScale, uniformScale);
            }
            sprite.Scale = baseScale * selection.ScaleMultiplier;
            if (sprite.SpriteFrames == frames) return;

            sprite.SpriteFrames = frames;
            sprite.Play("idle");
        }
        catch (System.Exception e)
        {
            MainFile.Logger.Error($"FormVisuals: deferred apply failed for {path}: {e.Message}");
        }
    }
}

[HarmonyPatch(typeof(NCombatRoom), "CreateAllyNodes")]
internal static class InitialFrameQualityPatch
{
    private static void Postfix(NCombatRoom __instance)
        => FormVisuals.ApplyRegisteredInitialFrames(__instance);
}

[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom._ExitTree))]
internal static class CombatFrameReleasePatch
{
    [HarmonyPostfix]
    private static void Postfix(NCombatRoom __instance)
        => FormVisuals.ReleaseCombatFrames(__instance);
}
