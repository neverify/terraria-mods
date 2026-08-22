using TerrariaModder.Core.Config;

namespace FunctionalSocialAccessories;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Client,
        Label("Enable Functional Social Accessories"),
        Description("Make social accessories function the same as regular accessories.")
    ]
    public bool EnableFunctionalSocialAccessories { get; set; } = true;
}
