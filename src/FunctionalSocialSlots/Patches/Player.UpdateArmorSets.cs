using HarmonyLib;
using Terraria;
using Terraria.DataStructures;

namespace FunctionalSocialSlots.Patches;

[HarmonyPatch(typeof(Player), nameof(Player.UpdateArmorSets))]
internal static class UpdateArmorSetsPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix(Player __instance)
    {
        if (!Mod.Instance.Config.FunctionalSocialArmorSlots)
            return;

        var queryContext = new ArmorSetBonus.QueryContext(__instance)
        {
            HeadItem = __instance.GetEffectiveArmor(10).type,
            BodyItem = __instance.GetEffectiveArmor(11).type,
            LegItem = __instance.GetEffectiveArmor(12).type,
        };

        ArmorSetBonuses.GetCompleteSet(queryContext)?.Effect(__instance);
    }
}
