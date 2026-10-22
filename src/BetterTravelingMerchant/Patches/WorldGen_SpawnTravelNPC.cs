using HarmonyLib;
using Terraria;

namespace BetterTravelingMerchant.Patches;

[HarmonyPatch(typeof(WorldGen), nameof(WorldGen.SpawnTravelNPC))]
internal static class SpawnTravelNPCPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix() => false;
}
