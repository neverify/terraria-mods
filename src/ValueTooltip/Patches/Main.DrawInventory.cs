using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using HarmonyLib;
using Terraria;
using Terraria.UI;
using ValueTooltip.Features;

namespace ValueTooltip.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(Main), "DrawInventory")]
internal static class DrawInventoryPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Postfix()
    {
        Player player = Main.player[Main.myPlayer];

        if (Mod.Instance.Config.ShowInventoryValue)
        {
            DrawInventoryValue(player.inventory.Take(Main.InventoryItemSlotsCount));
        }

        if (Mod.Instance.Config.ShowChestValue)
        {
            var container = player.GetCurrentContainer();
            if (container is null)
                return;
            DrawChestValue(container.item);
        }
    }

    private static void DrawInventoryValue(IEnumerable<Item> inventory)
    {
        ItemSlot.DrawMoney(
            Main.spriteBatch,
            "Inventory Value",
            Mod.Instance.Config.InventoryValueXPosition,
            Mod.Instance.Config.InventoryValueYPosition,
            [.. Coins.FromValue(Coins.GetValue(inventory)).Reverse()]
        );
    }

    private static void DrawChestValue(Item[] chest)
    {
        ItemSlot.DrawMoney(
            Main.spriteBatch,
            "Chest Value",
            Mod.Instance.Config.ChestValueXPosition,
            Mod.Instance.Config.ChestValueYPosition,
            [.. Coins.FromValue(Coins.GetValue(chest)).Reverse()],
            true
        );
    }
}
