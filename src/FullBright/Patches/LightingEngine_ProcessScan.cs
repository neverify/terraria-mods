using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

[HarmonyPatch(typeof(LightingEngine), "ProcessScan")]
internal static class ProcessScanPatch
{
    internal static Rectangle CachedActiveProcessedArea { get; private set; }

    internal static void Postfix(LightingEngine __instance)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return;

        CachedActiveProcessedArea = (Rectangle)s_activeProcessedAreaField.GetValue(__instance);
    }

    private static readonly FieldInfo s_activeProcessedAreaField = typeof(LightingEngine).GetField(
        "_activeProcessedArea",
        BindingFlags.NonPublic | BindingFlags.Instance
    );
}
