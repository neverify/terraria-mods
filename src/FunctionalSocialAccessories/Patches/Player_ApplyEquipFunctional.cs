using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Terraria;

namespace FunctionalSocialAccessories.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(Player))]
internal static class ApplyEquipFunctionalPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    [HarmonyPatch("ApplyEquipFunctional")]
    private static void Prefix(Player __instance, int itemSlot)
    {
        if (!Mod.Instance.Config.FunctionalSocialAccessorySlots)
            return;

        int socialSlot = itemSlot + 10;

        var socialEffectiveArmor = __instance.GetEffectiveArmor(socialSlot);

        if (!socialEffectiveArmor.IsAir && !(socialEffectiveArmor.expertOnly && !Main.expertMode))
        {
            ReversePatches.GrantPrefixBenefits(__instance, socialEffectiveArmor);
            ReversePatches.GrantArmorBenefits(__instance, socialEffectiveArmor);
            ReversePatches.ApplyEquipFunctional(__instance, itemSlot, socialEffectiveArmor);
        }
    }
}
