using FullBright.Features;
using Utils;

namespace FullBright;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "full-bright";
    public override string Name => "Fullbright";
    public override string Version => "1.1.1";

    public static void OnGameReady()
    {
        if (Instance is null)
            return;

        LightingQuality.SetQuality();
    }

    public void OnConfigChanged() => LightingQuality.SetQuality();
}
