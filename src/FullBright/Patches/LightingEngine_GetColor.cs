using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;
using Utils;

namespace FullBright.Patches;

[HarmonyPatch(typeof(LightingEngine), "GetColor")]
internal sealed class GetColorPatch : Patch<Mod>
{
    private static bool Prefix(int x, int y, ref Vector3 __result)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return true;

        Rectangle activeProcessedArea = ProcessScanPatch.CachedActiveProcessedArea;

        // Don't override color outside the active processed area.
        __result = activeProcessedArea.Contains(x, y)
            ? new Vector3(Mod.Instance.Config.Brightness)
            : Vector3.Zero;

        return false;
    }
}
