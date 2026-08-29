using Terraria.Testing;

namespace NoDamageVariance.Features;

internal static class DamageVariance
{
    internal static void Update() =>
        DebugOptions.NoDamageVar = Mod.Instance.Config.DisableDamageVariance;
}
