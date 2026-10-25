using HarmonyLib;
using Terraria.Testing;
using Terraria.Testing.ChatCommands;

namespace FullBright.Patches;

[HarmonyPatch(
    typeof(DebugCommandProcessor),
    nameof(DebugCommandProcessor.Process),
    [typeof(DebugMessage)]
)]
internal static class ProcessPatch
{
    private static bool Prepare => Mod.Instance is not null;

    private static void Prefix(DebugMessage message, out bool __state)
    {
        __state = DebugOptions.enableDebugCommands;

        if (message.CommandName is "map" or "clearmap")
            DebugOptions.enableDebugCommands = true;
    }

    private static void Postfix(bool __state) => DebugOptions.enableDebugCommands = __state;
}
