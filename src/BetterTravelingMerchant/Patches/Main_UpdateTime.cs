using System;
using System.Linq;
using HarmonyLib;
using Terraria;
using Utils;

namespace BetterTravelingMerchant.Patches;

[HarmonyPatch(typeof(Main), "UpdateTime")]
internal static class UpdateTimePatch
{
    private const int BaseSpawnChanceDenominator = 108000;

    private static bool Prepare() => Mod.Instance is not null;

    private static void Postfix()
    {
        if (!CanSpawn())
            return;

        double chance =
            Math.Max(Main.dayRate, 1)
            * Math.Max(Mod.Instance.Config.SpawnChanceMultiplier, 0.0f)
            / BaseSpawnChanceDenominator;

        if (Main.rand.NextDouble() < Math.Clamp(chance, 0.0, 1.0))
        {
            SpawnTravelNPC();
            NPC.travelNPC = true;
        }
    }

    private static bool CanSpawn()
    {
        bool timeCondition = !Main.IsFastForwardingTime() && Main.dayTime && Main.time < 27000.0;

        bool npcCondition = (
            from npc in Main.npc
            where npc.active && npc.townNPC && npc.type != 37 && npc.type != 453
            select npc
        )
            .Skip(1)
            .Any();

        return timeCondition && npcCondition;
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(WorldGen), nameof(WorldGen.SpawnTravelNPC))]
    private static void SpawnTravelNPC() => throw new NotImplementedException();
}
