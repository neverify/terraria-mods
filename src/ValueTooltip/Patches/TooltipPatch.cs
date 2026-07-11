using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace ValueTooltip.Patches;

internal static class TooltipPatch
{
    private const int PlatinumValue = 1000000;
    private const int GoldValue = 10000;
    private const int SilverValue = 100;

    internal static void Postfix(Item item, ref int numLines, string[] toolTipLine, Color[] lineColors)
    {
        if (!Mod.Config.ShowValueTooltips)
            return;

        if (item.value <= 0 || Main.npcShop > 0 || ItemID.Sets.CommonCoin[item.type])
            return;

        int sellValue = item.value / 5;
        int totalSellValue = sellValue * item.stack;

        if (sellValue == 0 || numLines >= toolTipLine.Length)
            return;

        toolTipLine[numLines] = GetValueTooltip(sellValue, item.stack);
        lineColors[numLines] = GetValueColor(totalSellValue);
        numLines++;
    }

    private static string GetValueTooltip(int sellValue, int stack)
    {
        string valueText = GetValueText(sellValue * stack);

        return stack <= 1 ? valueText : $"{valueText} ({GetValueText(sellValue)})";
    }

    private static string GetValueText(int value)
    {
        var coins = new[]
        {
            (Amount: value / PlatinumValue, Name: "Platinum"),
            (Amount: value % PlatinumValue / GoldValue, Name: "Gold"),
            (Amount: value % GoldValue / SilverValue, Name: "Silver"),
            (Amount: value % SilverValue, Name: "Copper"),
        };

        return string.Join(
            " ",
            coins
                .Where(coin => coin.Amount > 0)
                .Select(coin => $"{coin.Amount} {coin.Name}"));
    }

    private static Color GetValueColor(int value)
    {
        return value switch
        {
            int v when v >= PlatinumValue => Colors.CoinPlatinum,
            int v when v >= GoldValue => Colors.CoinGold,
            int v when v >= SilverValue => Colors.CoinSilver,
            _ => Colors.CoinCopper,
        };
    }
}
