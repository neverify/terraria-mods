using HarmonyLib;
using SelectOres.Patches;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;

namespace SelectOres;

public class Mod : IMod
{
    public string Id => "select-ores";
    public string Name => "Select Ores";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;
    private static Harmony s_harmony;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
        s_harmony = new Harmony("com.neverify.select-ores");
    }

    public static void OnGameReady()
    {
        if (s_harmony == null)
            return;

        var patcher = new Patcher(s_harmony, Log);

        patcher.Patch(
            typeof(WorldGen),
            "Reset",
            prefix: Patcher.GetHarmonyMethod(typeof(SmashAltarPatch), nameof(SmashAltarPatch.Prefix)),
            postfix: Patcher.GetHarmonyMethod(typeof(ResetPatch), nameof(ResetPatch.Postfix))
        );
    }

    public void OnWorldUnload() { }

    public void OnConfigChanged() { }

    public void Unload()
    {
        s_harmony.UnpatchAll("com.neverify.select-ores");
        Log.Info("Select Ores unloaded.");
    }
}
