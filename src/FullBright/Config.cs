using TerrariaModder.Core.Config;

namespace FullBright;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Brightness Override"), Description("Override the brightness of all tiles.")]
    public bool BrightnessOverride { get; set; } = true;

    [Client, Label("Brightness"), Description("The brightness at which to render tiles at."), Range(0f, 1f)]
    public float Brightness { get; set; } = 0.5f;

    [Client, Label("Disable Light Smoothing"), Description("Recommended with brightness override to improve performance. This option has no visual impact with brightness override on, since all tiles are the same brightness.")]
    public bool DisableLightSmoothing { get; set; } = true;
}
