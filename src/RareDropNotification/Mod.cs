using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace RareDropNotification;

public class Mod : IMod
{
    public string Id => "rare-drop-notification";
    public string Name => "Rare Drop Notification";
    public string Version => "1.0.0";

    internal static Mod Instance;

    internal ILogger Log { get; private set; }
    internal Config Config { get; private set; }

    public void Initialize(ModContext context)
    {
        Instance = this;

        Log = context.Logger;
        Config = context.GetConfig<Config>();
    }

    public void Unload() { }
}
