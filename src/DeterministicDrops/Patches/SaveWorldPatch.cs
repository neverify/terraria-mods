using HarmonyLib;
using Terraria.IO;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(WorldFile), nameof(WorldFile.SaveWorld))]
internal sealed class SaveWorldPatch : Patch<Mod>
{
    private static void Prefix() => Mod.Instance.DropStateStore?.Save();
}
