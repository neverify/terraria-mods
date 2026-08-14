using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace SettingsKeybind;

public class Mod : IMod
{
    public string Id => "settings-keybind";
    public string Name => "Settings Keybind";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static ModContext Context;
    internal static Config Config;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Context = context;
        Config = context.GetConfig<Config>();

        Keybinds.Register();
    }

    public void Unload() { }
}
