using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FullBright.CustomLighting;
using HarmonyLib;
using Terraria;
using Terraria.Graphics.Light;

namespace FullBright.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(Lighting), nameof(Lighting.Mode), MethodType.Setter)]
internal static class ModePatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(ref LightMode value)
    {
        if (Mod.Instance.Config.BrightnessOverride)
            value = LightMode.Color;
    }

    private static void Postfix()
    {
        if (Mod.Instance.Config.BrightnessOverride)
            s_activeEngineField.SetValue(null, FullBrightLightingEngine.Instance);
    }

    private static readonly FieldInfo s_activeEngineField = typeof(Lighting).GetField(
        "_activeEngine",
        BindingFlags.NonPublic | BindingFlags.Static
    );
}
