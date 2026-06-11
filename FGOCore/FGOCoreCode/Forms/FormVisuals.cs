using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace FGOCore.FGOCoreCode.Forms;

/// <summary>
/// Swaps a creature's combat sprite when it changes form. Each form declares its own
/// SpriteFrames resource via <see cref="FormPower.FramesPath"/> (same animation names),
/// so BaseLib's animation routing keeps working untouched.
/// Frames resources are heavy (tens of MB of textures); loading them synchronously on
/// first switch freezes the game for seconds. Character mods register every form's
/// path once (<see cref="RegisterFrames"/>); all registered paths are requested on
/// background threads at first use and pinned in a static cache, so the swap is instant.
/// </summary>
public static class FormVisuals
{
    private static readonly List<string> Registered = [];
    private static readonly Dictionary<string, SpriteFrames> Cache = [];
    private static readonly HashSet<string> Requested = [];

    /// <summary>Register form frame resources for background preloading (call at mod init).</summary>
    public static void RegisterFrames(params string[] paths)
    {
        foreach (var p in paths)
        {
            if (!Registered.Contains(p)) Registered.Add(p);
        }
    }

    /// <summary>Kicks off background loading of every registered frames resource (idempotent).</summary>
    public static void PreloadAll()
    {
        foreach (var path in Registered)
        {
            if (Cache.ContainsKey(path) || Requested.Contains(path)) continue;
            if (ResourceLoader.LoadThreadedRequest(path, "SpriteFrames", useSubThreads: true) == Error.Ok)
            {
                Requested.Add(path);
            }
        }
    }

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
        // First call per session warms every registered form in the background.
        PreloadAll();

        if (form.FramesPath == null) return;

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
