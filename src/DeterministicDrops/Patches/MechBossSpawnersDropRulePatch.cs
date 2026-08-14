using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(MechBossSpawnersDropRule), nameof(MechBossSpawnersDropRule.TryDroppingItem))]
internal static class MechBossSpawnersDropRulePatch
{
    public static bool Prefix(
        DropAttemptInfo info,
        MechBossSpawnersDropRule __instance,
        ref ItemDropAttemptResult __result
    ) =>
        TryDroppingItemPatchHelper.HandleDrop(
            info,
            __instance,
            DropProcessor.ProcessMechBossSpawnersDropRule,
            ref __result
        );
}
