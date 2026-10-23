using HarmonyLib;
using Terraria;
using Terraria.UI;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.UpdateEquips))]
internal static class UpdateEquipsPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(Player __instance)
    {
        if (!Mod.Instance.Config.FunctionalSocialArmorSlots)
            return;

        foreach (var item in ItemSlot.VanityArmorSlots(__instance))
        {
            if (!item.IsAir)
                ReversePatches.GrantArmorBenefits(__instance, item);
        }
    }
}
