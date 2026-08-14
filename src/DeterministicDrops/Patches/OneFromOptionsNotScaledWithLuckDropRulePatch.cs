using DeterministicDrops.DropSystem;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(OneFromOptionsNotScaledWithLuckDropRule),
    nameof(OneFromOptionsNotScaledWithLuckDropRule.TryDroppingItem)
)]
internal static class OneFromOptionsNotScaledWithLuckDropRulePatch
{
    public static bool Prefix(
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
