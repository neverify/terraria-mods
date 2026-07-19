using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace BetterTravelingMerchant.Patches;

internal static class SetupTravelShopPatch
{
    internal static void Postfix()
    {
        if (!Mod.Instance.Config.AdditionalItems)
            return;

        AddAllItems();
    }

    private static void AddAllItems()
    {
        AddItemSet(s_handOfCreationIngredients);
        AddItemSet(s_shellphoneIngredients);
    }

    private static void AddItemSet(HashSet<int> items)
    {
        int count = Array.IndexOf(Main.travelShop, 0);

        foreach (int itemId in items)
        {
            if (!Main.travelShop.Contains(itemId))
                Main.travelShop[count++] = itemId;
        }
    }

    private static readonly HashSet<int> s_handOfCreationIngredients =
    [
        ItemID.BrickLayer,
        ItemID.ExtendoGrip,
        ItemID.PaintSprayer,
        ItemID.PortableCementMixer,
    ];

    private static readonly HashSet<int> s_shellphoneIngredients =
    [
        ItemID.DPSMeter,
        ItemID.LifeformAnalyzer,
        ItemID.Stopwatch,
    ];
}
