using TerrariaModder.Core.Config;

namespace ValueTooltip;

public class Config : ModConfig
{
    public override int Version => 1;

    [Label("Show Value Tooltips"), Description("Show the sell value of items in their tooltip.")]
    public bool ShowValueTooltips { get; set; } = true;
}
