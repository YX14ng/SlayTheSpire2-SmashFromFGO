using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// Swaps a creature's combat sprite when it changes form. Each form declares its own
/// SpriteFrames resource via <see cref="FormPower.FramesPath"/> (same animation names),
/// so BaseLib's animation routing keeps working untouched.
/// Frames resources are heavy (hundreds of MB of textures per character). Each character
/// mod registers its OWN forms once (<see cref="RegisterFrames"/>, one call = one group);
/// when a creature enters combat we background-load that character's group in single-player.
/// Multiplayer loads only the current form: several FGO players preloading every alternate form
/// at once can still exhaust VRAM even though unrelated installed characters remain lazy.
/// <para>VRAM: do NOT preload every registered group. With N FGO character mods installed,
/// preloading all groups would pin N×(hundreds of frames) into VRAM at once even if you
/// only play one of them — that exhausted VRAM on normal GPUs (cards/intent failed to
/// render → "just health bars", or a hard crash with 3 characters). We scope the preload
/// to the group of the character actually fighting.</para>
/// </summary>
public static class FormVisuals
{
    // path -> the group (one character's full set of form paths) it belongs to.
    private static readonly Dictionary<string, string[]> GroupOf = [];
    private static readonly Dictionary<string, float> SpriteXOf = [];
    private static readonly Dictionary<string, float> SpriteYOf = [];
    private static readonly Dictionary<string, float> SpriteScaleOf = [];
    private static readonly Dictionary<string, SpriteFrames> Cache = [];
    private static readonly HashSet<string> Requested = [];
    // Paths whose background load failed once: never re-requested (a broken FramesPath would
    // otherwise re-request every frame → hundreds of "load failed" log lines per form switch).
    private static readonly HashSet<string> Failed = [];

    // Per-sprite generation stamp (Godot meta): a later Apply supersedes earlier ones so a slow
    // load can't overwrite the sprite with a stale form. Meta lives/dies with the node (no leak).
    private const string GenMeta = "fgo_form_gen";

    /// <summary>Register ONE character's form frame resources (call once at mod init).
    /// Each call is treated as a group: only the group of the character entering combat
    /// is preloaded, never every installed character's frames.</summary>
    public static void RegisterFrames(params string[] paths)
    {
        var group = paths.Where(p => !string.IsNullOrEmpty(p)).ToArray();
        foreach (var p in group) GroupOf[p] = group;
    }

    /// <summary>
    /// Registers a form group and the horizontal sprite pivot required by each frame set.
    /// FGO renders use asymmetric transparent canvases; because player sprites are mirrored,
    /// the compensation must also change when a form swaps to a differently framed model.
    /// </summary>
    public static void RegisterFramesWithSpriteX(params (string Path, float SpriteX)[] forms)
    {
        var valid = forms.Where(f => !string.IsNullOrEmpty(f.Path)).ToArray();
        var group = valid.Select(f => f.Path).ToArray();
        foreach (var form in valid)
        {
            GroupOf[form.Path] = group;
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
        var group = valid.Select(f => f.Path).ToArray();
        foreach (var form in valid)
        {
            GroupOf[form.Path] = group;
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
        var group = valid.Select(f => f.Path).ToArray();
        foreach (var form in valid)
        {
            GroupOf[form.Path] = group;
            SpriteXOf[form.Path] = form.SpriteX;
            SpriteYOf[form.Path] = form.SpriteY;
            SpriteScaleOf[form.Path] = form.UniformScale;
        }
    }

    /// <summary>Background-load the current form and, when allowed, its sibling forms.
    /// Multiplayer deliberately skips siblings to keep several FGO players within the VRAM budget.</summary>
    private static void PreloadGroup(string path, bool preloadAlternates)
    {
        var group = preloadAlternates && GroupOf.TryGetValue(path, out var registered)
            ? registered
            : [path];
        foreach (var p in group)
        {
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

    private static void TrackCombatAndEvict(string path, bool keepWholeGroup)
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
        if (keepWholeGroup && GroupOf.TryGetValue(path, out var group))
        {
            foreach (var p in group) _activePaths.Add(p);
        }
        else
        {
            _activePaths.Add(path);
        }

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

    public static void Apply(Creature creature, FormPower form)
    {
        if (form.FramesPath == null) return;

        // Single-player can afford to warm this character's complete form group for seamless swaps.
        // In co-op, each player loads only the form currently in use; a later switch remains async
        // and keeps the old sprite visible until ready instead of risking a black screen from VRAM
        // exhaustion when several FGO characters preload all of their alternatives together.
        var isMultiplayer = creature.Player?.RunState.Players.Count > 1;
        TrackCombatAndEvict(form.FramesPath, keepWholeGroup: !isMultiplayer);
        PreloadGroup(form.FramesPath, preloadAlternates: !isMultiplayer);

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite) return;

        // Stamp a new generation: a later Apply on this sprite supersedes this one, so an earlier
        // slow load can't resolve last and overwrite the sprite with the previous form (race).
        var gen = (sprite.HasMeta(GenMeta) ? sprite.GetMeta(GenMeta).AsInt32() : 0) + 1;
        sprite.SetMeta(GenMeta, gen);
        _ = ApplyWhenReadyAsync(sprite, form.FramesPath, gen);
    }

    /// <summary>
    /// Swaps the sprite as soon as the background load finishes, polling the scene's process_frame
    /// signal instead of ever blocking. Usually resolves on the first poll (group preloaded at
    /// combat start); the loop is the fallback for a switch on turn 1 before the load completes.
    /// </summary>
    private static async Task ApplyWhenReadyAsync(AnimatedSprite2D sprite, string path, int gen)
    {
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
            if (SpriteXOf.TryGetValue(path, out var spriteX))
            {
                position.X = spriteX;
                hasPositionOverride = true;
            }
            if (SpriteYOf.TryGetValue(path, out var spriteY))
            {
                position.Y = spriteY;
                hasPositionOverride = true;
            }
            if (hasPositionOverride) sprite.Position = position;
            if (SpriteScaleOf.TryGetValue(path, out var uniformScale))
            {
                sprite.Scale = new Vector2(uniformScale, uniformScale);
            }
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
