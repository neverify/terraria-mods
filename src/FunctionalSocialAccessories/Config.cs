using System.Collections.Generic;
using TerrariaModder.Core.Config;

namespace FunctionalSocialAccessories;

public class Config : ModConfig
{
    public override int Version => 2;

    [
        Client,
        Label("Functional Social Accessory Slots"),
        Description("Make accessories in social slots function the same as in normal slots.")
    ]
    public bool FunctionalSocialAccessorySlots { get; set; } = true;

    [
        Client,
        Label("Functional Social Armor Slots"),
        Description("Make armor in social slots function the same as in normal slots.")
    ]
    public bool FunctionalSocialArmorSlots { get; set; } = false;

    protected override void Migrate(Dictionary<string, object> raw, int fromVersion)
    {
        switch (fromVersion)
        {
            case 1:
                if (raw.TryGetValue("FunctionalSocialSlots", out object functionalSocialSlots))
                    FunctionalSocialAccessorySlots = (bool)functionalSocialSlots;
                break;
            default:
                break;
        }
    }
}
