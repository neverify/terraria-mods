using HarmonyLib;
using Terraria.Map;

namespace FullBright.Patches;

[HarmonyPatch(typeof(WorldMap), "UpdateLighting")]
internal static class UpdateLightingPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(int x, int y, WorldMap __instance)
    {
        if (!Mod.Instance.Config.MapLightingOverride)
            return true;

        byte brightness = (byte)(Mod.Instance.Config.MapLightingBrightness * byte.MaxValue);

        var mapTile = MapHelper.CreateMapTile(x, y, brightness);

        __instance.SetTile(x, y, ref mapTile);

        return false;
    }
}
