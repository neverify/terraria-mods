using System;
using HarmonyLib;
using Terraria;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(typeof(Player))]
internal static class ApplyEquipVanityPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    [HarmonyPatch("ApplyEquipVanity")]
    private static void Prefix(Player __instance, Item currentItem)
    {
        if (!Mod.Instance.Config.EnableFunctionalSocialAccessories)
            return;

        GrantPrefixBenefits(__instance, currentItem);
        ApplyEquipFunctional(__instance, 0, currentItem);
    }

    [HarmonyReversePatch]
    [HarmonyPatch("ApplyEquipFunctional")]
    private static void ApplyEquipFunctional(Player instance, int itemSlot, Item currentItem) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("GrantPrefixBenefits")]
    private static void GrantPrefixBenefits(Player instance, Item item) =>
        throw new NotImplementedException();
}
