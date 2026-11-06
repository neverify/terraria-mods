using System;
using System.Collections.Generic;
using TerrariaModder.Core.Config;

namespace FullBright;

public class Config : ModConfig
{
    public override int Version => 3;

    [Client, Label("Brightness Override"), Description("Enable the brightness override.")]
    public bool BrightnessOverride { get; set; } = true;

    [Client, Label("Brightness"), Description("The brightness to render tiles at."), Range(0f, 1f)]
    public float Brightness { get; set; } = 0.5f;

    [
        Client,
        Label("Lighting Padding"),
        Description("How much extra space to light around the viewport."),
        Range(0, 50)
    ]
    public int LightingPadding { get; set; } = 8;

    [Client, Label("Update Map Lighting"), Description("Whether to update map lighting.")]
    public bool UpdateMapLighting { get; set; } = true;

    [Client, Label("Map Brightness"), Description("The brightness of the map."), Range(0f, 1f)]
    public float MapBrightness { get; set; } = 1f;

    protected override void Migrate(Dictionary<string, object> raw, int fromVersion)
    {
        switch (fromVersion)
        {
            case 2:
                if (raw.TryGetValue("LightingAreaSizeOffset", out object lightingAreaSizeOffset))
                    LightingPadding = (int)lightingAreaSizeOffset + 28;
                if (raw.TryGetValue("MapLightingBrightness", out object mapLightingBrightness))
                    MapBrightness = Convert.ToSingle(mapLightingBrightness);
                break;
            default:
                break;
        }
    }
}
