using TerrariaModder.Core.Config;

namespace FullBright;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Brightness Override"), Description("Override the brightness of all tiles.")]
    public bool BrightnessOverride { get; set; } = true;

    [Client, Label("Brightness"), Description("The brightness at which to render tiles at."), Range(0f, 1f)]
    public float Brightness { get; set; } = 0.5f;

    [Client, Label("Use Low Quality Light Smoothing"), Description("Use a low quality light smoothing algorithm. Enabling this option when brightness override is enabled allows for a massive performance gain. This option has no visual impact when brightness override is on, since all tiles are the same brightness.")]
    public bool LightingOptimization { get; set; } = true;
}
