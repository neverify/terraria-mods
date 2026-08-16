using HarmonyLib;
using Terraria.Map;
using Utils;

namespace FullBright.Patches;

[HarmonyPatch(typeof(WorldMap), "UpdateLighting")]
internal sealed class UpdateLightingPatch : Patch<Mod>
{
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
