using Terraria;

namespace BetterTravelingMerchant.Patches;

internal static class UpdateTimePatch
{
    private const int BaseSpawnChanceDenominator = 108000;

    internal static void Postfix()
    {
        if (!(!Main.IsFastForwardingTime() && Main.dayTime && Main.time < 27000.0))
            return;

        if (Mod.Instance.Config.SpawnChanceMultiplier == 1.0f)
            return;

        int spawnChanceDenominator = (int)(BaseSpawnChanceDenominator / (Mod.Instance.Config.SpawnChanceMultiplier - 1.0f));

        if (Main.rand.Next(spawnChanceDenominator) == 0)
            WorldGen.SpawnTravelNPC();
    }
}
