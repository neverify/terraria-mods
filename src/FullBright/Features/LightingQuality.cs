using Terraria.Testing;

namespace FullBright.Features;

internal static class LightingQuality
{
    internal static void Update() =>
        DebugOptions.devLightTilesCheat = Mod.Instance.Config.DisableLightSmoothing;
}
