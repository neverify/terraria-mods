using HarmonyLib;
using Terraria.IO;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(WorldFile), nameof(WorldFile.SaveWorld))]
internal static class SaveWorldPatch
{
    internal static void Prefix() => Mod.Instance.DropStateStore?.Save();
}
