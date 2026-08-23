using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

[HarmonyPatch(typeof(LightingEngine), "GetColor")]
internal static class GetColorPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(int x, int y, ref Vector3 __result)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return true;

        // Don't override color outside the active processed area.
        __result = ProcessScanPatch.CachedActiveProcessedArea.Contains(x, y)
            ? new Vector3(Mod.Instance.Config.Brightness)
            : Vector3.Zero;

        return false;
    }
}
