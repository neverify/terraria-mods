using HarmonyLib;
using Terraria;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;
using Utils;
using ValueTooltip.Patches;

namespace ValueTooltip;

public class Mod : IMod
{
    public string Id => "value-tooltip";
    public string Name => "Value Tooltip";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;
    private static Harmony s_harmony;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
        s_harmony = new Harmony("com.neverify.value-tooltip");
    }

    public static void OnGameReady()
    {
        if (s_harmony == null)
            return;

        var patcher = new Patcher(s_harmony, Log);

        patcher.Patch(
            typeof(Main),
            "MouseText_DrawItemTooltip_GetLinesInfo",
            postfix: Patcher.GetHarmonyMethod(typeof(TooltipPatch), nameof(TooltipPatch.Postfix))
        );
    }

    public void OnConfigChanged() { }

    public void Unload()
    {
        s_harmony.UnpatchAll("com.neverify.value-tooltip");
        Log.Info("Unloaded.");
    }
}
