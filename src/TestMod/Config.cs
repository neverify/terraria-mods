using TerrariaModder.Core.Config;

namespace TestMod;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label(""), Description("")]
    public bool ConfigOption { get; set; } = true;
}
