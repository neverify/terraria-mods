using TerrariaModder.Core.Config;

namespace SelectOres;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Label("Override Generation"),
        Description(
            "Whether the selected ores are enforced on world generation and when smashing altars."
        )
    ]
    public bool OverrideGeneration { get; set; } = true;

    [Label("Tier 1 Ore"), Description("Which tier 1 ore to generate."), Options("Copper", "Tin")]
    public string Tier1Ore { get; set; } = "Copper";

    [Label("Tier 2 Ore"), Description("Which tier 2 ore to generate."), Options("Iron", "Lead")]
    public string Tier2Ore { get; set; } = "Iron";

    [
        Label("Tier 3 Ore"),
        Description("Which tier 3 ore to generate."),
        Options("Silver", "Tungsten")
    ]
    public string Tier3Ore { get; set; } = "Silver";

    [Label("Tier 4 Ore"), Description("Which tier 4 ore to generate."), Options("Gold", "Platinum")]
    public string Tier4Ore { get; set; } = "Gold";

    [
        Label("Tier 1 Hardmode Ore"),
        Description("Which hardmode tier 1 ore to generate."),
        Options("Cobalt", "Palladium")
    ]
    public string HardmodeTier1Ore { get; set; } = "Cobalt";

    [
        Label("Tier 2 Hardmode Ore"),
        Description("Which hardmode tier 2 ore to generate."),
        Options("Mythril", "Orichalcum")
    ]
    public string HardmodeTier2Ore { get; set; } = "Mythril";

    [
        Label("Tier 3 Hardmode Ore"),
        Description("Which hardmode tier 3 ore to generate."),
        Options("Adamantite", "Titanium")
    ]
    public string HardmodeTier3Ore { get; set; } = "Adamantite";
}
