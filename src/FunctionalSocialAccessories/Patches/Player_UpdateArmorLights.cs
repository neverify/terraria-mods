using HarmonyLib;
using Terraria;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.UpdateArmorLights))]
internal static class UpdateArmorLightsPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(Player __instance)
    {
        if (!Mod.Instance.Config.FunctionalSocialArmorSlots)
            return;

        var previousSlots = (__instance.head, __instance.body, __instance.legs);

        __instance.head = __instance.GetEffectiveArmor(10).headSlot;
        __instance.body = __instance.GetEffectiveArmor(11).bodySlot;
        __instance.legs = __instance.GetEffectiveArmor(12).legSlot;

        ReversePatches.UpdateArmorLights(__instance);

        (__instance.head, __instance.body, __instance.legs) = previousSlots;
    }
}
