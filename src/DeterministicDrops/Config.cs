using TerrariaModder.Core.Config;

namespace DeterministicDrops;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Enable Deterministic Drops"), Description("Enable the deterministic item drop system.")]
    public bool EnableDeterministicDrops { get; set; } = true;
}
