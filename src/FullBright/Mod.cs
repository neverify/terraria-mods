using FullBright.Features;
using Utils;

namespace FullBright;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "full-bright";
    public override string Name => "Fullbright";
    public override string Version => "1.1.3";

    protected override void Initialize() => LightingQuality.Update();

    public void OnConfigChanged() => LightingQuality.Update();
}
