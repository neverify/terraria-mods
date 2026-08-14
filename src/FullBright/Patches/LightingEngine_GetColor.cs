using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

[HarmonyPatch(typeof(LightingEngine), "GetColor")]
internal static class GetColorPatch
{
    internal static bool Prefix(int x, int y, ref Vector3 __result)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return true;

        Rectangle activeProcessedArea = ProcessScanPatch.CachedActiveProcessedArea;

        __result = activeProcessedArea.Contains(x, y)
            ? new Vector3(Mod.Instance.Config.Brightness)
            : Vector3.Zero;

        return false;
    }
}
