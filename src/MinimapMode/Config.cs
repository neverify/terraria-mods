using TerrariaModder.Core.Config;

namespace MinimapMode;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Force Minimap Mode"), Description("Force the configured minimap mode when loading into a world.")]
    public bool ForceMinimapMode { get; set; } = true;

    [Client, Label("Default Minimap Mode"), Description("The mode to set the minimap to when loading into a world."), Options("Hidden", "Minimap", "Overlay")]
    public string DefaultMinimapMode { get; set; } = "Minimap";
}
