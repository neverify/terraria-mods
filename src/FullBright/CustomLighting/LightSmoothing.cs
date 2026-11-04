using Terraria.Testing;

namespace FullBright.CustomLighting;

internal static class LightSmoothing
{
    internal static void Update() =>
        DebugOptions.devLightTilesCheat = Mod.Instance.Config.BrightnessOverride;
}
