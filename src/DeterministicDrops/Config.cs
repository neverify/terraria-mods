using TerrariaModder.Core.Config;

namespace DeterministicDrops;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Client,
        Label("Enable Deterministic NPC Drops"),
        Description("Enable the deterministic item drop system for NPCs.")
    ]
    public bool EnableDeterministicNpcDrops { get; set; } = true;

    [
        Client,
        Label("Enable Deterministic Treasure Bags"),
        Description("Enable the deterministic item drop system for treasure bags.")
    ]
    public bool EnableDeterministicTreasureBags { get; set; } = true;
}
