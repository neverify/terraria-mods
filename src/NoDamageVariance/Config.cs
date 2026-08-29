using TerrariaModder.Core.Config;

namespace NoDamageVariance;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Client,
        Label("Disable Damage Variance"),
        Description("Disable the ±15% damage variance for all sources of damage.")
    ]
    public bool DisableDamageVariance { get; set; } = true;
}
