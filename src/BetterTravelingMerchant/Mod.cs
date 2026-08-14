using TerrariaModder.Core;
using TerrariaModder.Core.Logging;

namespace BetterTravelingMerchant;

public class Mod : IMod
{
    public string Id => "better-traveling-merchant";
    public string Name => "Better Traveling Merchant";
    public string Version => "1.0.0";

    internal static Mod Instance { get; private set; }

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
