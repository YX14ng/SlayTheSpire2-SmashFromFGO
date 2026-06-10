using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace MashShielder.MashShielderCode.Powers.Forms;

/// <summary>
/// Swaps Mash's combat sprite when she changes form. Each form has its own
/// SpriteFrames resource (same animation names), so BaseLib's animation routing
/// keeps working untouched.
/// </summary>
public static class FormVisuals
{
    private static string FramesPath<T>() where T : FormPower => typeof(T).Name switch
    {
        nameof(OrtinaxFormPower) => $"{MainFile.ResPath}/character/mash_frames_ortinax.tres",
        nameof(PaladinFormPower) => $"{MainFile.ResPath}/character/mash_frames_paladin.tres",
        _ => $"{MainFile.ResPath}/character/mash_frames_base.tres"
    };

    public static void Apply<T>(Creature creature) where T : FormPower
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node?.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite) return;

        var frames = ResourceLoader.Load<SpriteFrames>(FramesPath<T>());
        if (frames == null)
        {
            MainFile.Logger.Error($"FormVisuals: could not load frames for {typeof(T).Name}");
            return;
        }
        if (sprite.SpriteFrames == frames) return;

        sprite.SpriteFrames = frames;
        sprite.Play("idle");
    }
}
