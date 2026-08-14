using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace RareDropNotification;

public class Mod : IMod
{
    public string Id => "rare-drop-notification";
    public string Name => "Rare Drop Notification";
    public string Version => "1.0.0";

    internal static ILogger Log;
    internal static Config Config;

    public void Initialize(ModContext context)
    {
        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public void Unload() { }
}
