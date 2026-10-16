using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TerrariaModder.Core.Config;

namespace RareDropNotification;

public class Config : ModConfig
{
    public override int Version => 2;

    [
        Client,
        Label("Show Notifications"),
        Description("Show a chat message when a rare item is dropped.")
    ]
    public bool ShowNotifications { get; set; } = true;

    [
        Client,
        Label("Notification Threshold"),
        Description("The minimum drop chance for the chat message to be sent."),
        Range(0f, 1f)
    ]
    public float NotificationThreshold { get; set; } = 0.1f;

    [
        Client,
        Label("Notification Format"),
        Description("Placeholders: {itemId}, {itemName}, {chance}.")
    ]
    public string NotificationFormat { get; set; } = "[i:{itemId}] {itemName} ({chance}%)";

    [Client, Label("Low Rarity Notification Color"), Description("Color of low rarity drops.")]
    public string LowRarityNotificationColor { get; set; } = "#22ffff";

    [Client, Label("High Rarity Notification Color"), Description("Color of high rarity drops.")]
    public string HighRarityNotificationColor { get; set; } = "#ddffff";

    internal static readonly Color FallbackColor = Color.White;

    protected override void Migrate(Dictionary<string, object> raw, int fromVersion)
    {
        switch (fromVersion)
        {
            case 1:
                if (raw.TryGetValue("Enabled", out var enabled))
                    ShowNotifications = (bool)enabled;
                if (raw.TryGetValue("DropNotificationFormat", out var notificationFormat))
                    NotificationFormat = (string)notificationFormat;
                if (raw.TryGetValue("NotificationColorLow", out var notificationColorLow))
                    LowRarityNotificationColor = (string)notificationColorLow;
                if (raw.TryGetValue("NotificationColorHigh", out var notificationColorHigh))
                    HighRarityNotificationColor = (string)notificationColorHigh;
                break;
            default:
                break;
        }
    }
}
