using Terraria.Testing;

namespace EnableDebugCommands.Features;

internal static class DebugCommands
{
    public static void Update() =>
        DebugOptions.enableDebugCommands = Mod.Instance.Config.EnableDebugCommands;
}
