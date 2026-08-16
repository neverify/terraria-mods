using Utils;

namespace SettingsKeybind;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "settings-keybind";
    public override string Name => "Settings Keybind";
    public override string Version => "1.0.2";

    protected override void Initialize() => Keybinds.Register();

    public void OnConfigChanged() { }
}
