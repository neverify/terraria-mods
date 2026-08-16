using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Utils;

namespace ValueTooltip.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.MouseText_DrawItemTooltip_GetLinesInfo))]
internal sealed class MouseText_DrawItemTooltip_GetLinesInfoPatch : Patch<Mod>
{
    private static void Postfix(
        Item item,
        ref int numLines,
        string[] toolTipLine,
        Color[] lineColors
    )
    {
        if (!Mod.Instance.Config.ShowValueTooltips)
            return;

        if (item.value <= 0 || Main.npcShop > 0 || ItemID.Sets.CommonCoin[item.type])
            return;

        int sellValue = item.value / 5;
        int totalSellValue = sellValue * item.stack;

        if (sellValue == 0 || numLines >= toolTipLine.Length)
            return;

        toolTipLine[numLines] = CreateValueTooltip(sellValue, item.stack);
        lineColors[numLines] = GetValueColor(totalSellValue);
        numLines++;
    }

    private static string CreateValueTooltip(int sellValue, int stack)
    {
        string valueText = GetValueText(sellValue * stack);

        return stack <= 1 ? valueText : $"{valueText} ({GetValueText(sellValue)})";
    }

    private static string GetValueText(int value)
    {
        var coins = new[]
        {
            (Amount: value / CoinValues.Platinum, Name: "Platinum"),
            (Amount: value % CoinValues.Platinum / CoinValues.Gold, Name: "Gold"),
            (Amount: value % CoinValues.Gold / CoinValues.Silver, Name: "Silver"),
            (Amount: value % CoinValues.Silver, Name: "Copper"),
        };

        return string.Join(
            " ",
            coins.Where(coin => coin.Amount > 0).Select(coin => $"{coin.Amount} {coin.Name}")
        );
    }

    private static Color GetValueColor(int value)
    {
        return value switch
        {
            int v when v >= CoinValues.Platinum => Colors.CoinPlatinum,
            int v when v >= CoinValues.Gold => Colors.CoinGold,
            int v when v >= CoinValues.Silver => Colors.CoinSilver,
            _ => Colors.CoinCopper,
        };
    }

    private static class CoinValues
    {
        public const int Platinum = 1000000;
        public const int Gold = 10000;
        public const int Silver = 100;
        public const int Copper = 1;
    }
}
