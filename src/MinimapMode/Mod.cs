using MinimapMode.Features;
using TerrariaModder.Core;
using Utils;

namespace MinimapMode;

public class Mod : ModBase<Mod, Config>, IModLifecycle
{
    public void OnWorldLoad()
    {
        if (!Config.ForceMinimapMode)
            return;

        SetMinimapMode.SetMode();
    }

    public void OnContentReady(ModContext context) { }

    public void OnWorldUnload() { }

    public void OnConfigChanged() { }
}
