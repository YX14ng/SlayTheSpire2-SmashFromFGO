using System;
using FGOCore.FGOCoreCode.Visuals;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace FGOCore.FGOCoreCode.Animation;

/// <summary>
/// Suaviza la presentacion de los sprites FGO sin cambiar la velocidad ni la duracion de las
/// animaciones que gobiernan el combate. Mantiene una copia muy tenue del fotograma anterior y
/// aplica una interpolacion subpixel al movimiento de reposo/accion; asi se reduce el parpadeo
/// propio de una secuencia raster sin retrasar comandos, VFX o cambios de forma.
/// </summary>
internal static class FgoAnimationSmoothing
{
    private const string ControllerName = "FgoAnimationSmoothing";

    private static readonly string[] ResourcePrefixes =
    [
        "res://MashShielder/",
        "res://MorganBerserker/",
        "res://ArtoriaCaster/",
        "res://MordredSaber/",
        "res://GilgameshArcher/",
        "res://OkitaSaber/",
        "res://OberonPretender/",
        "res://SiegfriedSaber/",
        "res://TiamatBeast/",
        "res://KagetoraLancer/",
        "res://ShutenDouji/",
        "res://AstolfoRider/",
    ];

    internal static T Prepare<T>(T root) where T : Node
    {
        var profile = FgoVisualQuality.GetAnimationProfile();
        if (root.FindChild("Sprite", recursive: true, owned: false) is not AnimatedSprite2D sprite
            || sprite.SpriteFrames is null
            || !IsFgoResource(sprite.SpriteFrames.ResourcePath)
            || !profile.Enabled
            || sprite.GetNodeOrNull<Node>(ControllerName) is not null)
        {
            return root;
        }

        sprite.AddChild(new FgoSpriteMotion { Name = ControllerName, Profile = profile });
        return root;
    }

    private static bool IsFgoResource(string path)
    {
        foreach (var prefix in ResourcePrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.Ordinal)) return true;
        }

        return false;
    }
}

/// <summary>
/// Capa puramente visual. Trabaja sobre <see cref="AnimatedSprite2D.Offset"/>, que FormVisuals no
/// modifica, por lo que los pivotes X/Y y la escala de cada forma siguen siendo la fuente de verdad.
/// </summary>
internal partial class FgoSpriteMotion : Node2D
{
    internal FgoAnimationProfile Profile { get; init; }

    private AnimatedSprite2D _sprite = null!;
    private Sprite2D _previousFrame = null!;
    private Texture2D? _lastTexture;
    private Vector2 _baseOffset;
    private Vector2 _motionOffset;
    private Vector2 _lastVisualOffset;
    private float _idleTime;
    private float _blendAlpha;

    public override void _Ready()
    {
        _sprite = GetParent<AnimatedSprite2D>();
        _baseOffset = _sprite.Offset;
        _lastVisualOffset = _sprite.Offset;
        _lastTexture = CurrentTexture();

        _previousFrame = new Sprite2D
        {
            Name = "PreviousFrame",
            Centered = _sprite.Centered,
            FlipH = _sprite.FlipH,
            FlipV = _sprite.FlipV,
            Offset = _sprite.Offset,
            ZIndex = -1,
            Visible = false,
        };
        AddChild(_previousFrame);

        _sprite.AnimationChanged += OnAnimationChanged;
        _sprite.FrameChanged += OnFrameChanged;
        _sprite.AnimationFinished += OnAnimationFinished;
    }

    public override void _ExitTree()
    {
        if (!GodotObject.IsInstanceValid(_sprite)) return;
        _sprite.AnimationChanged -= OnAnimationChanged;
        _sprite.FrameChanged -= OnFrameChanged;
        _sprite.AnimationFinished -= OnAnimationFinished;
    }

    public override void _Process(double delta)
    {
        if (_sprite.SpriteFrames is null) return;

        var seconds = (float)delta;
        var target = TargetMotionOffset(seconds);
        var response = 1f - Mathf.Exp(-Profile.OffsetResponse * seconds);
        _motionOffset = _motionOffset.Lerp(target, response);
        _sprite.Offset = _baseOffset + _motionOffset;
        _lastVisualOffset = _sprite.Offset;

        if (_blendAlpha <= 0f || !_previousFrame.Visible) return;
        _blendAlpha = Mathf.MoveToward(_blendAlpha, 0f, seconds / Profile.BlendFadeSeconds);
        _previousFrame.SelfModulate = new Color(1f, 1f, 1f, _blendAlpha);
        if (_blendAlpha <= 0f) _previousFrame.Visible = false;
    }

    private Vector2 TargetMotionOffset(float delta)
    {
        var animation = _sprite.Animation;
        if (animation == "idle")
        {
            _idleTime = (_idleTime + delta) % Profile.IdleCycleSeconds;
            var phase = _idleTime / Profile.IdleCycleSeconds * Mathf.Tau;
            // Siempre hacia arriba: los pies nunca atraviesan el plano de suelo.
            return new Vector2(Mathf.Sin(phase * 0.5f) * 0.25f, -(1f + Mathf.Sin(phase)) * 0.75f)
                * Profile.MotionScale;
        }

        var progress = AnimationProgress();
        var ease = Mathf.Sin(progress * Mathf.Pi);
        if (animation == "attack") return new Vector2(ease * 3.0f, -ease * 0.5f) * Profile.MotionScale;
        if (animation == "cast") return new Vector2(0f, -ease * 2.2f) * Profile.MotionScale;
        if (animation == "hurt") return new Vector2(-ease * 2.0f, 0f) * Profile.MotionScale;
        return Vector2.Zero;
    }

    private float AnimationProgress()
    {
        var frameCount = _sprite.SpriteFrames?.GetFrameCount(_sprite.Animation) ?? 0;
        return frameCount <= 1 ? 1f : Mathf.Clamp((float)_sprite.Frame / (frameCount - 1), 0f, 1f);
    }

    private void OnAnimationChanged()
    {
        ShowPreviousFrame(Profile.ActionBlendAlpha);
        _lastTexture = CurrentTexture();
        if (_sprite.Animation == "idle") _idleTime = 0f;
    }

    private void OnFrameChanged()
    {
        var current = CurrentTexture();
        if (_lastTexture is not null && current != _lastTexture)
        {
            ShowPreviousFrame(_sprite.Animation == "idle" ? Profile.IdleBlendAlpha : Profile.ActionBlendAlpha);
        }
        _lastTexture = current;
    }

    private void OnAnimationFinished()
    {
        if (_sprite.Animation != "die" && _sprite.SpriteFrames?.HasAnimation("idle") == true)
        {
            _sprite.Play("idle");
        }
    }

    private void ShowPreviousFrame(float alpha)
    {
        if (_lastTexture is null) return;
        _previousFrame.Texture = _lastTexture;
        _previousFrame.Centered = _sprite.Centered;
        _previousFrame.FlipH = _sprite.FlipH;
        _previousFrame.FlipV = _sprite.FlipV;
        _previousFrame.Offset = _lastVisualOffset;
        _blendAlpha = alpha;
        _previousFrame.SelfModulate = new Color(1f, 1f, 1f, alpha);
        _previousFrame.Visible = true;
    }

    private Texture2D? CurrentTexture()
    {
        var frames = _sprite.SpriteFrames;
        if (frames is null || !frames.HasAnimation(_sprite.Animation)) return null;
        var count = frames.GetFrameCount(_sprite.Animation);
        return count == 0 ? null : frames.GetFrameTexture(_sprite.Animation, Math.Clamp(_sprite.Frame, 0, count - 1));
    }
}

[HarmonyPatch(typeof(NCreatureVisuals), nameof(NCreatureVisuals._Ready))]
internal static class CreatureAnimationSmoothingPatch
{
    private static void Postfix(NCreatureVisuals __instance)
        => FgoAnimationSmoothing.Prepare(__instance);
}
