using NoDamageVariance.Features;
using Terraria.Testing;
using Utils;

namespace NoDamageVariance;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize() =>
        DebugOptions.NoDamageVar = Instance.Config.DisableDamageVariance;

    public void OnConfigChanged() => DamageVariance.Update();
}
