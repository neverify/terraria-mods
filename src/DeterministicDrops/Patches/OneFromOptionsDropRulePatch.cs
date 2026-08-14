using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(OneFromOptionsDropRule), nameof(OneFromOptionsDropRule.TryDroppingItem))]
internal static class OneFromOptionsDropRulePatch
{
    public static bool Prefix(
        DropAttemptInfo info,
        OneFromOptionsDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessOneFromOptionsDropRule,
            ref __result
        );
}
