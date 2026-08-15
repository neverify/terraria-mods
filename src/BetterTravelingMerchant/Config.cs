using TerrariaModder.Core.Config;

namespace BetterTravelingMerchant;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Label("Spawn Chance Multiplier"),
        Description("Multiply the chance of the traveling merchant spawning each tick."),
        Range(1.0f, 100.0f)
    ]
    public float SpawnChanceMultiplier { get; set; } = 1.0f;

    [
        Label("Additional Items"),
        Description("Make the Traveling Merchant always sell the items configured below.")
    ]
    public bool AdditionalItems { get; set; } = true;

    [
        Label("Hand of Creation Ingredients"),
        Description("Always sell the Hand of Creation ingredients.")
    ]
    public bool HandOfCreationIngredients { get; set; } = true;

    [Label("Shellphone Ingredients"), Description("Always sell the Shellphone ingredients.")]
    public bool ShellphoneIngredients { get; set; } = true;
}
