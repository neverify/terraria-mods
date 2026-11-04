using Terraria;

namespace FullBright.CustomLighting;

internal static class LightingController
{
    public static void Refresh() => Lighting.Mode = Lighting.Mode;
}
