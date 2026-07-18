using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

internal static class ProcessScanPatch
{
    private static readonly FieldInfo s_activeProcessedAreaField = typeof(LightingEngine)
        .GetField("_activeProcessedArea", BindingFlags.NonPublic | BindingFlags.Instance);

    internal static Rectangle CachedActiveProcessedArea { get; private set; }

    internal static void Postfix(LightingEngine __instance)
    {
        if (!Mod.Config.BrightnessOverride)
            return;

        CachedActiveProcessedArea = (Rectangle)s_activeProcessedAreaField.GetValue(__instance);
    }
}
