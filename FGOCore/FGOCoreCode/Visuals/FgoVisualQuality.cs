using BaseLib.Config;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Microsoft.Win32;
using System.Runtime.Versioning;

namespace FGOCore.FGOCoreCode.Visuals;

internal enum VisualQualityMode
{
    Auto,
    Balanced,
    High,
}

internal enum AnimationSmoothingMode
{
    Disabled,
    Standard,
    Enhanced,
}

internal enum FrameQualityTier
{
    Balanced,
    High,
}

[ConfigHoverTipsByDefault]
internal sealed class FgoVisualConfig : SimpleModConfig
{
    public static VisualQualityMode VisualQuality { get; set; } = VisualQualityMode.Auto;

    public static AnimationSmoothingMode AnimationSmoothing { get; set; } = AnimationSmoothingMode.Standard;
}

internal readonly record struct FrameResourceSelection(
    string LogicalPath,
    string ResourcePath,
    float ScaleMultiplier);

internal readonly record struct FgoAnimationProfile(
    bool Enabled,
    float IdleBlendAlpha,
    float ActionBlendAlpha,
    float BlendFadeSeconds,
    float OffsetResponse,
    float IdleCycleSeconds,
    float MotionScale);

/// <summary>
/// Resolves local presentation quality without changing character model IDs or gameplay state.
/// A quality choice is frozen for the lifetime of a combat so changing the option cannot swap a
/// heavy frame resource while animations are already running.
/// </summary>
internal static class FgoVisualQuality
{
    private const ulong Gibibyte = 1024UL * 1024UL * 1024UL;
    private const ulong AutoMinimumVram = 8UL * Gibibyte;
    private const ulong AutoMinimumHeadroom = 3UL * Gibibyte;
    private const float HighToBalancedScale = 768f / 1024f;
    private const string CharacterPathSegment = "/character/";
    private const string HighQualitySubdirectory = "quality_high/";

    private static ulong _activeCombatId;
    private static bool _hasActiveCombat;
    private static FrameQualityTier _activeCombatTier;
    private static bool _hasDedicatedVramReading;
    private static ulong _dedicatedVram;

    internal static FrameQualityTier GetFrameTier(bool isMultiplayer)
    {
        var room = NCombatRoom.Instance;
        if (room is null) return DetermineFrameTier(isMultiplayer, logDecision: false);

        var combatId = room.GetInstanceId();
        if (_hasActiveCombat && combatId == _activeCombatId) return _activeCombatTier;

        _hasActiveCombat = true;
        _activeCombatId = combatId;
        _activeCombatTier = DetermineFrameTier(isMultiplayer, logDecision: true);
        return _activeCombatTier;
    }

    internal static FrameResourceSelection ResolveFrames(string logicalPath, FrameQualityTier tier)
    {
        if (tier != FrameQualityTier.High)
        {
            return new FrameResourceSelection(logicalPath, logicalPath, 1f);
        }

        var markerIndex = logicalPath.IndexOf(CharacterPathSegment, StringComparison.Ordinal);
        if (markerIndex < 0)
        {
            return new FrameResourceSelection(logicalPath, logicalPath, 1f);
        }

        var insertAt = markerIndex + CharacterPathSegment.Length;
        var highPath = logicalPath.Insert(insertAt, HighQualitySubdirectory);
        return ResourceLoader.Exists(highPath, "SpriteFrames")
            ? new FrameResourceSelection(logicalPath, highPath, HighToBalancedScale)
            : new FrameResourceSelection(logicalPath, logicalPath, 1f);
    }

    internal static FgoAnimationProfile GetAnimationProfile()
        => FgoVisualConfig.AnimationSmoothing switch
        {
            AnimationSmoothingMode.Disabled => new FgoAnimationProfile(false, 0f, 0f, 0.055f, 18f, 3.2f, 1f),
            AnimationSmoothingMode.Enhanced => new FgoAnimationProfile(true, 0.13f, 0.20f, 0.075f, 16f, 3.4f, 0.90f),
            _ => new FgoAnimationProfile(true, 0.10f, 0.16f, 0.055f, 18f, 3.2f, 1f),
        };

    private static FrameQualityTier DetermineFrameTier(bool isMultiplayer, bool logDecision)
    {
        var configured = FgoVisualConfig.VisualQuality;
        if (configured == VisualQualityMode.Balanced)
        {
            LogDecision(logDecision, FrameQualityTier.Balanced, "manual");
            return FrameQualityTier.Balanced;
        }

        if (configured == VisualQualityMode.High)
        {
            LogDecision(logDecision, FrameQualityTier.High, "manual");
            return FrameQualityTier.High;
        }

        var adapterType = RenderingServer.GetVideoAdapterType();
        var totalVram = GetDedicatedVram();
        var usedVram = RenderingServer.GetRenderingInfo(RenderingServer.RenderingInfo.VideoMemUsed);
        var availableVram = totalVram > usedVram ? totalVram - usedVram : 0UL;
        var useHigh = !isMultiplayer
            && adapterType == RenderingDevice.DeviceType.DiscreteGpu
            && totalVram >= AutoMinimumVram
            && availableVram >= AutoMinimumHeadroom;

        var tier = useHigh ? FrameQualityTier.High : FrameQualityTier.Balanced;
        if (logDecision)
        {
            var totalGib = totalVram / (double)Gibibyte;
            var availableGib = availableVram / (double)Gibibyte;
            MainFile.Logger.Info(
                $"Visual quality: {tier} (auto, adapter={adapterType}, VRAM={totalGib:F1} GiB, available={availableGib:F1} GiB, multiplayer={isMultiplayer})");
        }
        return tier;
    }

    private static ulong GetDedicatedVram()
    {
        if (_hasDedicatedVramReading) return _dedicatedVram;

        _dedicatedVram = OperatingSystem.IsWindows()
            ? GetWindowsDedicatedVram(RenderingServer.GetVideoAdapterName())
            : 0UL;
        _hasDedicatedVramReading = true;
        return _dedicatedVram;
    }

    [SupportedOSPlatform("windows")]
    private static ulong GetWindowsDedicatedVram(string activeAdapterName)
    {
        if (string.IsNullOrWhiteSpace(activeAdapterName)) return 0UL;

        try
        {
            using var videoRoot = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Video");
            if (videoRoot is null) return 0UL;

            ulong matchingMemory = 0UL;
            foreach (var adapterKeyName in videoRoot.GetSubKeyNames())
            {
                using var adapterKey = videoRoot.OpenSubKey(adapterKeyName);
                if (adapterKey is null) continue;

                foreach (var instanceKeyName in adapterKey.GetSubKeyNames())
                {
                    using var instanceKey = adapterKey.OpenSubKey(instanceKeyName);
                    if (instanceKey is null) continue;

                    var driverName = instanceKey.GetValue("DriverDesc") as string;
                    var adapterName = instanceKey.GetValue("HardwareInformation.AdapterString") as string;
                    if (!AdapterNamesMatch(activeAdapterName, driverName)
                        && !AdapterNamesMatch(activeAdapterName, adapterName))
                    {
                        continue;
                    }

                    matchingMemory = Math.Max(matchingMemory, ReadRegisteredVram(instanceKey));
                }
            }
            return matchingMemory;
        }
        catch (System.Exception e)
        {
            MainFile.Logger.Warn($"Visual quality: could not read dedicated VRAM ({e.Message}); using Balanced");
            return 0UL;
        }
    }

    [SupportedOSPlatform("windows")]
    private static ulong ReadRegisteredVram(RegistryKey key)
    {
        var wideValue = ToUnsigned(key.GetValue("HardwareInformation.qwMemorySize"));
        if (wideValue > 0UL) return wideValue;

        var legacyValue = ToUnsigned(key.GetValue("HardwareInformation.MemorySize"));
        if (legacyValue == 0UL) return 0UL;

        // Drivers commonly store bytes here, while the documented legacy contract uses MiB.
        // Values small enough to be a plausible MiB count are therefore expanded; larger values
        // are treated as bytes. The QWORD value above remains authoritative whenever available.
        return legacyValue <= 65_536UL ? legacyValue * 1024UL * 1024UL : legacyValue;
    }

    private static ulong ToUnsigned(object? value)
        => value switch
        {
            ulong unsignedLong => unsignedLong,
            long signedLong when signedLong > 0 => (ulong)signedLong,
            uint unsignedInt => unsignedInt,
            int signedInt => unchecked((uint)signedInt),
            _ => 0UL,
        };

    private static bool AdapterNamesMatch(string activeName, string? registeredName)
    {
        if (string.IsNullOrWhiteSpace(registeredName)) return false;
        return activeName.Contains(registeredName, StringComparison.OrdinalIgnoreCase)
            || registeredName.Contains(activeName, StringComparison.OrdinalIgnoreCase);
    }

    private static void LogDecision(bool enabled, FrameQualityTier tier, string source)
    {
        if (enabled) MainFile.Logger.Info($"Visual quality: {tier} ({source})");
    }
}
