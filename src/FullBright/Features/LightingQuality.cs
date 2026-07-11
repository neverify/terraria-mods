using Terraria.Testing;

namespace FullBright.Features;

internal static class LightingQuality
{
    internal static void SetQuality() => DebugOptions.devLightTilesCheat = Mod.Config.LightingOptimization;
}
