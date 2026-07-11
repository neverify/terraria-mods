using Terraria;

namespace MinimapMode.Features;

internal sealed class SetMinimapMode
{
    internal static void SetMode()
    {
        Main.mapStyle = Mod.Config.DefaultMinimapMode switch
        {
            "Hidden" => 0,
            "Minimap" => 1,
            "Overlay" => 2,
            _ => 1
        };
    }
}
