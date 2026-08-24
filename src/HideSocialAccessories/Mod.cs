using Utils;

namespace HideSocialAccessories;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "hide-social-accessories";
    public override string Name => "Hide Social Accessories";
    public override string Version => "1.0.0";

    public void OnConfigChanged() { }
}
