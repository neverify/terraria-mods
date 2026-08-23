using TerrariaModder.Core.Config;

namespace FunctionalSocialAccessories;

public class Config : ModConfig
{
    public override int Version => 1;

    [
        Client,
        Label("Functional Social Slots"),
        Description("Make accessories in social slots function the same as in normal slots.")
    ]
    public bool FunctionalSocialSlots { get; set; } = true;
}
