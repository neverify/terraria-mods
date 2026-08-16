using HarmonyLib;
using Terraria.IO;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(WorldFile), nameof(WorldFile.SaveWorld))]
internal sealed class SaveWorldPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix() => Mod.Instance.DropStateStore?.Save();
}
