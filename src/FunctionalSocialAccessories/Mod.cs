using Utils;

namespace FunctionalSocialAccessories;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "functional-social-accessories";
    public override string Name => "Functional Social Accessories";
    public override string Version => "1.0.3";

    public void OnConfigChanged() { }
}
