using System;
using HarmonyLib;
using Terraria;

namespace FunctionalSocialSlots.Patches;

[HarmonyPatch(typeof(Player))]
internal static class ReversePatches
{
    [HarmonyReversePatch]
    [HarmonyPatch("ApplyEquipFunctional")]
    public static void ApplyEquipFunctional(Player instance, int itemSlot, Item currentItem) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("GrantPrefixBenefits")]
    public static void GrantPrefixBenefits(Player instance, Item item) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("GrantArmorBenefits")]
    public static void GrantArmorBenefits(Player instance, Item item) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("UpdateArmorLights")]
    public static void UpdateArmorLights(Player instance) => throw new NotImplementedException();
}
