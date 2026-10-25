using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Terraria.Testing;
using Terraria.Testing.ChatCommands;

namespace FullBright.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(DebugCommandProcessor), MethodType.Constructor)]
internal static class DebugCommandProcessorCtorPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(out bool __state)
    {
        __state = DebugOptions.enableDebugCommands;
        DebugOptions.enableDebugCommands = true;
    }

    private static void Postfix(bool __state) => DebugOptions.enableDebugCommands = __state;
}
