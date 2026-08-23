using System;
using HarmonyLib;
using Terraria;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(typeof(Player))]
internal static class ApplyEquipFunctionalPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    [HarmonyPatch("ApplyEquipFunctional")]
    private static void Prefix(Player __instance, int itemSlot)
    {
        if (!Mod.Instance.Config.EnableFunctionalSocialAccessories)
            return;

        int socialSlot = itemSlot + 10;

        var socialEffectiveArmor = __instance.GetEffectiveArmor(socialSlot);

        if (!socialEffectiveArmor.IsAir && !(socialEffectiveArmor.expertOnly && !Main.expertMode))
        {
            GrantPrefixBenefits(__instance, socialEffectiveArmor);
            GrantArmorBenefits(__instance, socialEffectiveArmor);
            ApplyEquipFunctional(__instance, itemSlot, socialEffectiveArmor);
        }
    }

    [HarmonyReversePatch]
    [HarmonyPatch("ApplyEquipFunctional")]
    private static void ApplyEquipFunctional(Player instance, int itemSlot, Item currentItem) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("GrantPrefixBenefits")]
    private static void GrantPrefixBenefits(Player instance, Item item) =>
        throw new NotImplementedException();

    [HarmonyReversePatch]
    [HarmonyPatch("GrantArmorBenefits")]
    private static void GrantArmorBenefits(Player instance, Item item) =>
        throw new NotImplementedException();
}
