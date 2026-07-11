using Microsoft.Xna.Framework;
using TerrariaModder.Core.Config;

namespace RareDropNotification;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Show Notifications"), Description("Show a chat message when the conditions are met.")]
    public bool Enabled { get; set; } = true;

    [Client, Label("Notification Threshold"), Description("The threshold for displaying notifications"), Range(0f, 1f)]
    public float NotificationThreshold { get; set; } = 0.1f;

    [Client, Label("Drop Notification Format"), Description("The format of the chat message. The following placeholders are available: `{itemId}`, `{itemName}`, `{chance}`.")]
    public string DropNotificationFormat { get; set; } = "[i:{itemId}] {itemName} ({chance})%";

    [Client, Label("Low Rarity Notification Color"), Description("The gradient edge for low rarity drops.")]
    public string NotificationColorLow { get; set; } = "#22ffff";

    [Client, Label("High Rarity Notification Color"), Description("The gradient edge for high rarity drops.")]
    public string NotificationColorHigh { get; set; } = "#ddffff";

    internal static readonly Color FallbackColor = Color.White;
}
