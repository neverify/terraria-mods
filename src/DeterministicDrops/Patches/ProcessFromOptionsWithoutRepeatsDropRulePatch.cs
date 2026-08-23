using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(FromOptionsWithoutRepeatsDropRule),
    nameof(FromOptionsWithoutRepeatsDropRule.TryDroppingItem)
)]
internal static class FromOptionsWithoutRepeatsDropRulePatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(
        DropAttemptInfo info,
        FromOptionsWithoutRepeatsDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessFromOptionsWithoutRepeatsDropRule,
            ref __result
        );
}
