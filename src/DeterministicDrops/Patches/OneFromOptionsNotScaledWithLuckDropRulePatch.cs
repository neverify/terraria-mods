using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(
    typeof(OneFromOptionsNotScaledWithLuckDropRule),
    nameof(OneFromOptionsNotScaledWithLuckDropRule.TryDroppingItem)
)]
internal sealed class OneFromOptionsNotScaledWithLuckDropRulePatch
{
    private static bool Prepare() => Mod.Instance is not null;

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
