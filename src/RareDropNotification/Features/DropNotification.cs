using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Utils;

namespace RareDropNotification.Features;

internal static class DropNotification
{
    internal static void HandleDrop(CommonDrop drop)
    {
        float chance = (float)drop.chanceNumerator / drop.chanceDenominator;

        if (chance > Mod.Instance.Config.NotificationThreshold)
            return;

        string itemName = ContentSamples.ItemsByType[drop.itemId].Name;

        string message = PlaceholderFormatter.Format(
            Mod.Instance.Config.NotificationFormat,
            ("itemId", drop.itemId.ToString()),
            ("itemName", itemName),
            ("chance", (chance * 100).ToString("F2"))
        );

        Color color = GetColor(chance, Mod.Instance.Config.NotificationThreshold);

        ShowNotification(message, color);
    }

    private static void ShowNotification(string message, Color color) =>
        Main.NewText(message, color);

    private static Color GetColor(float chance, float threshold)
    {
        Color lowColor = ParseHex(Mod.Instance.Config.LowRarityNotificationColor);
        Color highColor = ParseHex(Mod.Instance.Config.HighRarityNotificationColor);

        // Calculate interpolation factor.
        float t = threshold > 0 ? chance / threshold : 1f;
        t = Math.Max(0f, Math.Min(1f, t));

        // Apply linear interpolation.
        return Color.Lerp(lowColor, highColor, t);
    }

    private static Color ParseHex(string hex)
    {
        if (string.IsNullOrEmpty(hex) || hex.Length != 7 || hex[0] != '#')
        {
            Mod.Instance.Log.Warn($"Invalid hex color format: {hex}, using fallback color.");
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
            Mod.Instance.Log.Warn($"Failed to parse hex color: {hex}, using fallback color.");
            return Config.FallbackColor;
        }
    }
}
