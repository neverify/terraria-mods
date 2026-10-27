using TerrariaModder.Core.Config;

namespace FullBright;

public class Config : ModConfig
{
    public override int Version => 2;

    [
        Client,
        Label("Brightness Override"),
        Description("Enable the brightness override. Only works with the \"color\" lighting mode.")
    ]
    public bool BrightnessOverride { get; set; } = true;

    [Client, Label("Brightness"), Description("The brightness to render tiles at."), Range(0f, 1f)]
    public float Brightness { get; set; } = 0.5f;

    [
        Client,
        Label("Lighting Area Size Offset"),
        Description("An offset to decrease the size of the lighting area by."),
        Range(-100, 0)
    ]
    public int LightingAreaSizeOffset { get; set; } = -20;

    [Client, Label("Disable Light Smoothing"), Description("Disable vanilla light smoothing.")]
    public bool DisableLightSmoothing { get; set; } = true;

    [
        Client,
        Label("Map Lighting Override"),
        Description("Override the brightness of map lighting.")
    ]
    public bool MapLightingOverride { get; set; } = true;

    [
        Client,
        Label("Map Lighting Brightness"),
        Description("The brightness to set map tiles to."),
        Range(0f, 1f)
    ]
    public float MapLightingBrightness { get; set; } = 1f;
}
