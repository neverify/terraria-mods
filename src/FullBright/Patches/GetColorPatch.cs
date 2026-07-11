using Microsoft.Xna.Framework;

namespace FullBright.Patches;

internal static class GetColorPatch
{
#pragma warning disable IDE1006 // Naming Styles
    internal static bool Prefix(int x, int y, ref Vector3 __result)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (!Mod.Config.BrightnessOverride)
            return true;

        Rectangle activeProcessedArea = ProcessScanPatch.CachedActiveProcessedArea;

        __result = activeProcessedArea.Contains(x, y) switch
        {
            true => new Vector3(Mod.Config.Brightness),
            false => Vector3.Zero
        };

        return false;
    }
}
