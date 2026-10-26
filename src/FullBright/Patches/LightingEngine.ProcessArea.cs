using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

[HarmonyPatch(typeof(LightingEngine), nameof(LightingEngine.ProcessArea))]
internal static class ProcessAreaPatch
{
    private const int BaseOffset = 28;

    private static bool Prepare() => Mod.Instance is not null;

    public static Rectangle Area { get; private set; }

    private static void Prefix(Rectangle area)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return;

        int offset = Mod.Instance.Config.LightingAreaSizeOffset + BaseOffset;
        area.Inflate(offset, offset);

        Area = area;
    }
}
