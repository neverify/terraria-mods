using Microsoft.Xna.Framework;
using TerrariaModder.Core.Config;

namespace RareDropNotification;

public class Config : ModConfig
{
    public override int Version => 1;

    [Label("Show Notifications"), Description("Show a chat message when a rare item is dropped.")]
    public bool Enabled { get; set; } = true;

    [
        Label("Notification Threshold"),
        Description("The minimum drop chance for the chat message to be sent."),
        Range(0f, 1f)
    ]
    public float NotificationThreshold { get; set; } = 0.1f;

    [
        Label("Drop Notification Format"),
        Description(
            "The format of the chat message. The following placeholders are available: `{itemId}`, `{itemName}`, `{chance}`."
        )
    ]
    public string DropNotificationFormat { get; set; } = "[i:{itemId}] {itemName} ({chance})%";

    [Label("Low Rarity Notification Color"), Description("The gradient edge for low rarity drops.")]
    public string NotificationColorLow { get; set; } = "#22ffff";

    [
        Label("High Rarity Notification Color"),
        Description("The gradient edge for high rarity drops.")
    ]
    public string NotificationColorHigh { get; set; } = "#ddffff";

    internal static readonly Color FallbackColor = Color.White;
}
