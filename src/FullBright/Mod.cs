using FullBright.Features;
using Utils;

namespace FullBright;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize() => LightingQuality.Update();

    public void OnConfigChanged() => LightingQuality.Update();
}
