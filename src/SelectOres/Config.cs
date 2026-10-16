using System.Collections.Generic;
using TerrariaModder.Core.Config;

namespace SelectOres;

public class Config : ModConfig
{
    public override int Version => 2;

    [
        Client,
        Label("Override World Generation Ores"),
        Description("Whether the selected ores are enforced during world generation.")
    ]
    public bool OverrideWorldGenerationOres { get; set; } = true;

    [
        Client,
        Label("Tier 1 Ore"),
        Description("Which tier 1 ore to generate."),
        Options("Copper", "Tin")
    ]
    public string Tier1Ore { get; set; } = "Copper";

    [
        Client,
        Label("Tier 2 Ore"),
        Description("Which tier 2 ore to generate."),
        Options("Iron", "Lead")
    ]
    public string Tier2Ore { get; set; } = "Iron";

    [
        Client,
        Label("Tier 3 Ore"),
        Description("Which tier 3 ore to generate."),
        Options("Silver", "Tungsten")
    ]
    public string Tier3Ore { get; set; } = "Silver";

    [
        Client,
        Label("Tier 4 Ore"),
        Description("Which tier 4 ore to generate."),
        Options("Gold", "Platinum")
    ]
    public string Tier4Ore { get; set; } = "Gold";

    [
        Client,
        Label("Override Altar Smashing Ores"),
        Description("Whether the selected ores are enforced when smashing altars.")
    ]
    public bool OverrideAltarSmashingOres { get; set; } = true;

    [
        Client,
        Label("Tier 1 Hardmode Ore"),
        Description("Which hardmode tier 1 ore to generate."),
        Options("Cobalt", "Palladium")
    ]
    public string HardmodeTier1Ore { get; set; } = "Cobalt";

    [
        Client,
        Label("Tier 2 Hardmode Ore"),
        Description("Which hardmode tier 2 ore to generate."),
        Options("Mythril", "Orichalcum")
    ]
    public string HardmodeTier2Ore { get; set; } = "Mythril";

    [
        Client,
        Label("Tier 3 Hardmode Ore"),
        Description("Which hardmode tier 3 ore to generate."),
        Options("Adamantite", "Titanium")
    ]
    public string HardmodeTier3Ore { get; set; } = "Adamantite";

    protected override void Migrate(Dictionary<string, object> raw, int fromVersion)
    {
        switch (fromVersion)
        {
            case 1:
                if (raw.TryGetValue("OverrideGeneration", out var overrideGeneration))
                {
                    OverrideWorldGenerationOres = (bool)overrideGeneration;
                    OverrideAltarSmashingOres = (bool)overrideGeneration;
                }
                break;
            default:
                break;
        }
    }
}
