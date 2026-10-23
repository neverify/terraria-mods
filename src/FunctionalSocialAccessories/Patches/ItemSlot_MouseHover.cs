using HarmonyLib;
using Terraria;
using Terraria.UI;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(
    typeof(ItemSlot),
    nameof(ItemSlot.MouseHover),
    [typeof(Item[]), typeof(int), typeof(int)]
)]
internal static class MouseHoverPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Postfix(int context)
    {
        if (
            context switch
            {
                9 when Mod.Instance.Config.FunctionalSocialArmorSlots => true,
                11 when Mod.Instance.Config.FunctionalSocialAccessorySlots => true,
                _ => false,
            }
        )
        {
            Main.HoverItem.social = false;
        }
    }
}
