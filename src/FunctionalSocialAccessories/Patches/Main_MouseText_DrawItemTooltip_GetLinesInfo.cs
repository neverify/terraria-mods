using HarmonyLib;
using Terraria;

namespace FunctionalSocialAccessories.Patches;

[HarmonyPatch(typeof(Main), nameof(Main.MouseText_DrawItemTooltip_GetLinesInfo))]
internal static class MouseText_DrawItemTooltip_GetLinesInfoPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(Item item, out bool __state)
    {
        if (!Mod.Instance.Config.FunctionalSocialSlots)
        {
            __state = false;
            return;
        }

        __state = item.social;
        item.social = false;
    }

    private static void Postfix(Item item, bool __state)
    {
        if (!Mod.Instance.Config.FunctionalSocialSlots)
            return;

        item.social = __state;
    }
}
