using MinimapMode.Features;
using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace MinimapMode;

public class Mod : IMod, IModLifecycle
{
    public string Id => "minimap-mode";
    public string Name => "Minimap Mode";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public void OnWorldLoad()
    {
        if (!Config.ForceMinimapMode)
            return;

        SetMinimapMode.SetMode();
    }

    public void OnContentReady(ModContext context) { }

    public void OnWorldUnload() { }

    public void OnConfigChanged() { }

    public void Unload() => Log.Info("Unloaded.");
}
