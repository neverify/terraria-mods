using System;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;

namespace RareDropNotification.Features;

internal static class DropNotification
{
    internal static void HandleDrop(CommonDrop drop)
    {
        float chance = (float)drop.chanceNumerator / drop.chanceDenominator;

        if (chance > Mod.Config.NotificationThreshold)
            return;

        string itemName = ContentSamples.ItemsByType[drop.itemId].Name;

        // Format notification message
        string message = FormatNotificationMessage(Mod.Config.DropNotificationFormat, drop.itemId, itemName, chance);

        // Get notification color
        Color color = GetColor(chance, Mod.Config.NotificationThreshold);

        ShowNotification(message, color);
    }

    private static void ShowNotification(string message, Color color) => Main.NewText(message, color);

    private static string FormatNotificationMessage(string format, int itemId, string itemName, float chance)
    {
        float chancePercent = chance * 100;

        string result = Regex.Replace(
            format,
            @"\{(\w+)\}",
            m =>
            {
                string placeholder = m.Groups[1].Value;

                return placeholder switch
                {
                    "itemId" => itemId.ToString(),
                    "itemName" => itemName,
                    "chance" => chancePercent.ToString("F2"),
                    _ => m.Value
                };
            }
        );

        return result;
    }

    private static Color GetColor(float chance, float threshold)
    {
        Color lowColor = ParseHex(Mod.Config.NotificationColorLow);
        Color highColor = ParseHex(Mod.Config.NotificationColorHigh);

        float t = threshold > 0 ? chance / threshold : 1f;
        t = Math.Max(0f, Math.Min(1f, t));

        return Color.Lerp(lowColor, highColor, t);
    }

    private static Color ParseHex(string hex)
    {
        if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#')
        {
            Mod.Log.Warn($"Invalid hex color format: {hex}, using fallback color.");
            return Config.FallbackColor;
        }

        try
        {
            byte r = Convert.ToByte(hex.Substring(1, 2), 16);
            byte g = Convert.ToByte(hex.Substring(3, 2), 16);
            byte b = Convert.ToByte(hex.Substring(5, 2), 16);
            return new Color(r, g, b);
        }
        catch
        {
            Mod.Log.Warn($"Failed to parse hex color: {hex}, using fallback color.");
            return Config.FallbackColor;
        }
    }
}
