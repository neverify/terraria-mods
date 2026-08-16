using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(FromOptionsWithoutRepeatsDropRule),
    nameof(FromOptionsWithoutRepeatsDropRule.TryDroppingItem)
)]
internal sealed class FromOptionsWithoutRepeatsDropRulePatch : Patch<Mod>
{
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
