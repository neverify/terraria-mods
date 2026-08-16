using HarmonyLib;
using Terraria;

namespace BetterTravelingMerchant.Patches;

[HarmonyPatch(typeof(Main), "UpdateTime")]
internal sealed class MainPatch
{
    private const int BaseSpawnChanceDenominator = 108000;

    private static bool Prepare() => Mod.Instance != null;

    private static void Postfix()
    {
        if (Mod.Instance.Config.SpawnChanceMultiplier == 1.0f)
            return;

        if (!(!Main.IsFastForwardingTime() && Main.dayTime && Main.time < 27000.0))
            return;

        int spawnChanceDenominator = (int)(
            BaseSpawnChanceDenominator / (Mod.Instance.Config.SpawnChanceMultiplier - 1.0f)
        );

        if (Main.rand.Next(spawnChanceDenominator) == 0)
            WorldGen.SpawnTravelNPC();
    }
}
