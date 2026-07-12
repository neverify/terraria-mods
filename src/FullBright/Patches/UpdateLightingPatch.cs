using Terraria.Map;

namespace FullBright.Patches;

internal static class UpdateLightingPatch
{
#pragma warning disable IDE1006 // Naming Styles
    internal static bool Prefix(int x, int y, WorldMap __instance)
#pragma warning restore IDE1006 // Naming Styles
    {
        if (!Mod.Config.MapLightingOverride)
            return true;

        byte brightness = (byte)(Mod.Config.MapLightingBrightness * byte.MaxValue);

        var mapTile = MapHelper.CreateMapTile(x, y, brightness);

        __instance.SetTile(x, y, ref mapTile);

        return false;
    }
}
