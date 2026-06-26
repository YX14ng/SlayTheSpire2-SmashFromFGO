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
/// when a creature enters combat we background-load ONLY that character's group and pin it
/// in a static cache, so the form swap is instant.
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
    private static readonly Dictionary<string, SpriteFrames> Cache = [];
    private static readonly HashSet<string> Requested = [];

    /// <summary>Register ONE character's form frame resources (call once at mod init).
    /// Each call is treated as a group: only the group of the character entering combat
    /// is preloaded, never every installed character's frames.</summary>
    public static void RegisterFrames(params string[] paths)
    {
        var group = paths.Where(p => !string.IsNullOrEmpty(p)).ToArray();
        foreach (var p in group) GroupOf[p] = group;
    }

    /// <summary>Background-load only the forms that belong to the same character as
    /// <paramref name="path"/> (idempotent).</summary>
    private static void PreloadGroup(string path)
    {
        if (!GroupOf.TryGetValue(path, out var group)) group = [path];
        foreach (var p in group)
        {
            if (Cache.ContainsKey(p) || Requested.Contains(p)) continue;
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

        if (!Requested.Contains(path))
        {
            // Defensive: Apply already kicks the load via PreloadGroup, but if we got here cold,
            // request it in the background and report "not ready yet" without blocking.
            if (ResourceLoader.LoadThreadedRequest(path, "SpriteFrames", useSubThreads: true) == Error.Ok)
                Requested.Add(path);
            return null;
        }

        var status = ResourceLoader.LoadThreadedGetStatus(path);
        if (status != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            if (status is ResourceLoader.ThreadLoadStatus.Failed or ResourceLoader.ThreadLoadStatus.InvalidResource)
            {
                Requested.Remove(path);
                MainFile.Logger.Error($"FormVisuals: background load failed for {path}");
            }
            return null; // still loading → caller retries next frame, never blocks the sim thread.
        }

        var frames = ResourceLoader.LoadThreadedGet(path) as SpriteFrames;
        Requested.Remove(path);
        if (frames != null) Cache[path] = frames;
        return frames;
    }

    public static void Apply(Creature creature, FormPower form)
    {
        if (form.FramesPath == null) return;

        // Warm ONLY this character's forms in the background (not every installed mod's). Because
        // this fires for the base form at combat start too, the whole group (incl. mid-combat forms
        // like Ortinax) is usually fully loaded by the time the player actually switches.
        PreloadGroup(form.FramesPath);

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite) return;

        ApplyWhenReady(sprite, form.FramesPath);
    }

    /// <summary>
    /// Swaps the sprite as soon as the background load finishes, polling the scene's process_frame
    /// signal instead of ever blocking. Usually resolves on the first poll (group preloaded at
    /// combat start); the loop is the fallback for a switch on turn 1 before the load completes.
    /// </summary>
    private static async void ApplyWhenReady(AnimatedSprite2D sprite, string path)
    {
        try
        {
            var frames = GetFrames(path);
            if (frames == null)
            {
                var tree = sprite.GetTree();
                // Bound the wait (~20s @60fps) so a failed/never-finishing load can't spin forever.
                for (var i = 0; i < 1200 && frames == null; i++)
                {
                    if (tree == null || !GodotObject.IsInstanceValid(sprite)) return;
                    await tree.ToSignal(tree, SceneTree.SignalName.ProcessFrame);
                    frames = GetFrames(path);
                }
            }

            if (frames == null || !GodotObject.IsInstanceValid(sprite)) return;
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
