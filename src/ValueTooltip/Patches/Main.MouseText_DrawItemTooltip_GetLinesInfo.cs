using System.Linq;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Utils;
using ValueTooltip.Features;

namespace ValueTooltip.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.MouseText_DrawItemTooltip_GetLinesInfo))]
internal static class MouseText_DrawItemTooltip_GetLinesInfoPatch
{
    private static bool Prepare() => Mod.Instance is not null;

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

        long sellValue = item.value / 5;
        long totalSellValue = sellValue * item.stack;

        if (sellValue == 0 || numLines >= toolTipLine.Length)
            return;

        toolTipLine[numLines] = CreateValueTooltip(sellValue, item.stack);
        lineColors[numLines] = GetValueColor(totalSellValue);
        numLines++;
    }

    private static string CreateValueTooltip(long sellValue, int stack)
    {
        string value = GetValueText(sellValue);
        string totalValue = GetValueText(sellValue * stack);

        string format =
            stack <= 1
                ? Mod.Instance.Config.SingularItemTooltipFormat
                : Mod.Instance.Config.StackedItemTooltipFormat;

        return PlaceholderFormatter.Format(format, ("value", value), ("totalValue", totalValue));
    }

    public static string GetValueText(long value)
    {
        var coinAmounts = Coins.FromValue(value);
        string[] coinNames = ["Platinum", "Gold", "Silver", "Copper"];

        return string.Join(
            " ",
            coinAmounts
                .Select((coin, index) => new { coin, name = coinNames[index] })
                .Where(entry => entry.coin > 0)
                .Select(entry => $"{entry.coin} {entry.name}")
        );
    }

    private static Color GetValueColor(long value)
    {
        return value switch
        {
            long v when v >= Coins.Platinum => Colors.CoinPlatinum,
            long v when v >= Coins.Gold => Colors.CoinGold,
            long v when v >= Coins.Silver => Colors.CoinSilver,
            _ => Colors.CoinCopper,
        };
    }
}
