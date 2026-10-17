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

    [
        Client,
        Label("Show Inventory Value"),
        Description("Show the combined value of all items in the inventory.")
    ]
    public bool ShowInventoryValue { get; set; } = true;

    [
        Client,
        Label("Inventory Value X Position"),
        Description("The X position of the inventory value display.")
    ]
    public int InventoryValueXPosition { get; set; } = 505;

    [
        Client,
        Label("Inventory Value Y Position"),
        Description("The Y position of the inventory value display.")
    ]
    public int InventoryValueYPosition { get; set; } = -10;

    [
        Client,
        Label("Show Chest Value"),
        Description("Show the combined value of all items in an open chest.")
    ]
    public bool ShowChestValue { get; set; } = true;

    [
        Client,
        Label("Chest Value X Position"),
        Description("The X position of the chest value display.")
    ]
    public int ChestValueXPosition { get; set; } = 225;

    [
        Client,
        Label("Chest Value Y Position"),
        Description("The Y position of the chest value display.")
    ]
    public int ChestValueYPosition { get; set; } = 395;
}
