using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace SettingsKeybind;

public class Mod : IMod
{
    public string Id => "settings-keybind";
    public string Name => "Settings Keybind";
    public string Version => "1.0.0";

    internal static Mod Instance { get; private set; }

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }
    internal ModContext Context { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
        Context = context;

        Keybinds.Register();
    }

    public void OnConfigChanged() { }

    public void Unload() { }
}
