using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.OpenBossBag))]
internal sealed class OpenBossBagPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(int type, Player __instance)
    {
        if (!Mod.Instance.Config.EnableDeterministicTreasureBags)
            return true;

        var itemSource = __instance.GetItemSource_OpenItem(type);
        var dropContexts = BossBagDatabase.GetDropContexts(type);

        foreach (var dropContext in dropContexts)
        {
            var dropResults = DropProcessor.ProcessDrop(dropContext, Mod.Instance.DropStateStore);

            foreach (var dropResult in dropResults)
                __instance.QuickSpawnItem(itemSource, dropResult.ItemId, dropResult.Amount);
        }

        var coinAmount = BossBagDatabase.GetCoinAmount(type);
        SpawnCoins(__instance, itemSource, coinAmount);

        return false;
    }

    private static void SpawnCoins(
        Player player,
        IEntitySource itemSource,
        BossBagDatabase.CoinAmount coinAmount
    )
    {
        player.QuickSpawnItem(itemSource, ItemID.CopperCoin, coinAmount.Copper);
        player.QuickSpawnItem(itemSource, ItemID.SilverCoin, coinAmount.Silver);
        player.QuickSpawnItem(itemSource, ItemID.GoldCoin, coinAmount.Gold);
        player.QuickSpawnItem(itemSource, ItemID.PlatinumCoin, coinAmount.Platinum);
    }
}
