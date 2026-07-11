using FullBright.Features;
using FullBright.Patches;
using HarmonyLib;
using Terraria.Graphics.Light;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace FullBright;

public class Mod : IMod
{
    public string Id => "full-bright";
    public string Name => "Fullbright";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;
    private static Harmony s_harmony;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
        s_harmony = new Harmony("com.neverify.full-bright");
    }

    public static void OnGameReady()
    {
        if (s_harmony == null)
            return;

        var patcher = new Patcher(s_harmony, Log);

        patcher.Patch(
            typeof(LightingEngine),
            "ProcessScan",
            postfix: Patcher.GetHarmonyMethod(typeof(ProcessScanPatch), nameof(ProcessScanPatch.Postfix))
        );

        patcher.Patch(
            typeof(LightingEngine),
            "GetColor",
            prefix: Patcher.GetHarmonyMethod(typeof(GetColorPatch), nameof(GetColorPatch.Prefix))
        );

        LightingQuality.SetQuality();
    }

    public void Unload()
    {
        s_harmony.UnpatchAll("com.neverify.full-bright");
        Log.Info("Unloaded.");
    }

#pragma warning disable CA1822 // Mark members as static
    public void OnConfigChanged() => LightingQuality.SetQuality();
#pragma warning restore CA1822 // Mark members as static
}
