using Utils;

namespace SettingsKeybind;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize() => Keybinds.Register();

    public void OnConfigChanged() { }
}
