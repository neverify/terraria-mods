using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FullBright.CustomLighting;
using HarmonyLib;
using Terraria;
using Terraria.Graphics;

namespace FullBright.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(Main), "RenderToTargets")]
internal static class RenderToTargetsPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private enum RenderState
    {
        Tile,
        Background,
        Misc,
    }

    private static void NextState(ref RenderState renderState) =>
        renderState = (RenderState)(
            (int)(renderState + 1) % Enum.GetValues(typeof(RenderState)).Length
        );

    private static RenderState s_renderState;

    private static bool Prefix(Main __instance)
    {
        if (!Mod.Instance.Config.BrightnessOverride)
            return true;

        Lighting.LightTiles(Main.GetAreaToLight());

        switch (s_renderState)
        {
            case RenderState.Tile:
                UpdateContent(s_tileTargetField, () => RenderTiles(__instance));
                break;
            case RenderState.Background:
                UpdateContent(s_backgroundTargetField, () => RenderBackground(__instance));
                break;
            case RenderState.Misc:
                UpdateContent(s_tile2TargetField, () => RenderTiles2(__instance));
                UpdateContent(s_wallTargetField, () => RenderWallsAndBlacks(__instance));
                UpdateContent(s_waterTargetField, () => RenderWater(__instance));
                UpdateContent(s_backWaterTargetField, () => RenderBackgroundWater(__instance));

                FullBrightLightingEngine.Instance.UpdateMinimap();
                Main.UpdateSceneMetrics();
                break;
            default:
                break;
        }

        NextState(ref s_renderState);
        Main.renderCount = 0;

        return false;
    }

    private static void UpdateContent(FieldInfo targetField, Action render)
    {
        object target = targetField.GetValue(null);
        _ = s_updateContentMethod.Invoke(target, [render]);
    }

    private static readonly MethodInfo s_updateContentMethod =
        typeof(WorldSceneLayerTarget).GetMethod("UpdateContent");

    private static readonly FieldInfo s_backWaterTargetField = typeof(Main).GetField(
        "backWaterTarget",
        BindingFlags.NonPublic | BindingFlags.Static
    );

    private static readonly FieldInfo s_waterTargetField = typeof(Main).GetField(
        "waterTarget",
        BindingFlags.Public | BindingFlags.Static
    );

    private static readonly FieldInfo s_backgroundTargetField = typeof(Main).GetField(
        "backgroundTarget",
        BindingFlags.NonPublic | BindingFlags.Static
    );

    private static readonly FieldInfo s_wallTargetField = typeof(Main).GetField(
        "wallTarget",
        BindingFlags.NonPublic | BindingFlags.Static
    );

    private static readonly FieldInfo s_tileTargetField = typeof(Main).GetField(
        "tileTarget",
        BindingFlags.NonPublic | BindingFlags.Static
    );

    private static readonly FieldInfo s_tile2TargetField = typeof(Main).GetField(
        "tile2Target",
        BindingFlags.NonPublic | BindingFlags.Static
    );

    [HarmonyReversePatch]
    [HarmonyPatch("RenderWater")]
    private static void RenderWater(Main instance) => throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("RenderBackgroundWater")]
    private static void RenderBackgroundWater(Main instance) => throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("RenderBackground")]
    private static void RenderBackground(Main instance) => throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("RenderWallsAndBlacks")]
    private static void RenderWallsAndBlacks(Main instance) => throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("RenderTiles")]
    private static void RenderTiles(Main instance) => throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("RenderTiles2")]
    private static void RenderTiles2(Main instance) => throw new NotImplementedException();
}
