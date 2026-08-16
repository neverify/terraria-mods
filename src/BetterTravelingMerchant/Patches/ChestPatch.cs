using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Terraria;
using Terraria.ID;
using Utils;

namespace BetterTravelingMerchant.Patches;

[HarmonyPatch(typeof(Chest), nameof(Chest.SetupTravelShop))]
internal sealed class ChestPatch : Patch<Mod>
{
    private static void Postfix()
    {
        if (!Mod.Instance.Config.AdditionalItems)
            return;

        AddAllItems();
    }

    private static void AddAllItems()
    {
        if (Mod.Instance.Config.HandOfCreationIngredients)
            AddItemSet(s_handOfCreationIngredients);

        if (Mod.Instance.Config.ShellphoneIngredients)
            AddItemSet(s_shellphoneIngredients);
    }

    private static void AddItemSet(HashSet<int> items)
    {
        // Find the first available slot.
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
