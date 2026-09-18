using TerrariaModder.Core.Config;

namespace EnableDebugCommands;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Enable Debug Commands"), Description("Enable the vanilla debug commands.")]
    public bool EnableDebugCommands { get; set; } = true;
}
