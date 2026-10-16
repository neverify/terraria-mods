using TerrariaModder.Core.Config;

namespace ValueTooltip;

public class Config : ModConfig
{
    public override int Version => 2;

    [
        Client,
        Label("Show Value Tooltips"),
        Description("Show the sell value of items in their tooltip.")
    ]
    public bool ShowValueTooltips { get; set; } = true;

    [Client, Label("Singular Item Tooltip Format"), Description("Placeholders: {value}.")]
    public string SingularItemTooltipFormat { get; set; } = "{value}";

    [
        Client,
        Label("Stacked Item Tooltip Format"),
        Description("Placeholders: {value}, {totalValue}.")
    ]
    public string StackedItemTooltipFormat { get; set; } = "{totalValue} ({value})";
}
