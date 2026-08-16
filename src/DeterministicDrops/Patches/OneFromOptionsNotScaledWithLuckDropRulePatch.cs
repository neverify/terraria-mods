using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(OneFromOptionsNotScaledWithLuckDropRule),
    nameof(OneFromOptionsNotScaledWithLuckDropRule.TryDroppingItem)
)]
internal sealed class OneFromOptionsNotScaledWithLuckDropRulePatch : Patch<Mod>
{
    private static bool Prefix(
        DropAttemptInfo info,
        OneFromOptionsNotScaledWithLuckDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessOneFromOptionsNotScaledWithLuckDropRule,
            ref __result
        );
}
