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

    private static SpriteFrames? GetFrames(string path)
    {
        if (Cache.TryGetValue(path, out var cached)) return cached;

        SpriteFrames? frames = null;
        if (Requested.Contains(path))
        {
            // Blocks only for whatever the background thread hasn't finished yet.
            frames = ResourceLoader.LoadThreadedGet(path) as SpriteFrames;
            Requested.Remove(path);
        }
        frames ??= ResourceLoader.Load<SpriteFrames>(path);

        if (frames != null) Cache[path] = frames;
        return frames;
    }

    public static void Apply(Creature creature, FormPower form)
    {
        if (form.FramesPath == null) return;

        // Warm ONLY this character's forms in the background (not every installed mod's).
        PreloadGroup(form.FramesPath);

        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite) return;

        var frames = GetFrames(form.FramesPath);
        if (frames == null)
        {
            MainFile.Logger.Error($"FormVisuals: could not load frames at {form.FramesPath}");
            return;
        }
        if (sprite.SpriteFrames == frames) return;

        sprite.SpriteFrames = frames;
        sprite.Play("idle");
    }
}
