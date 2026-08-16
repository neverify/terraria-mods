using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(MechBossSpawnersDropRule), nameof(MechBossSpawnersDropRule.TryDroppingItem))]
internal sealed class MechBossSpawnersDropRulePatch
{
    private static bool Prepare() => Mod.Instance != null;

    private static bool Prefix(
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
