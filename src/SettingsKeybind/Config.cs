using TerrariaModder.Core.Config;

namespace SettingsKeybind;

public class Config : ModConfig
{
    public override int Version => 1;

    [Client, Label("Hide Settings Button"), Description("Hide the vanilla settings button at the bottom right of the screen.")]
    public bool HideSettingsButton { get; set; } = false;
}
