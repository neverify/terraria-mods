using Terraria.Map;

namespace FullBright.Patches;

internal static class UpdateLightingPatch
{
    internal static bool Prefix(int x, int y, WorldMap __instance)
    {
        if (!Mod.Config.MapLightingOverride)
            return true;

        byte brightness = (byte)(Mod.Config.MapLightingBrightness * byte.MaxValue);

        var mapTile = MapHelper.CreateMapTile(x, y, brightness);

        __instance.SetTile(x, y, ref mapTile);

        return false;
    }
}
