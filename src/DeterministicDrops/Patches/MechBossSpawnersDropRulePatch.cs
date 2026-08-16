using DeterministicDrops.DropEngine;
using HarmonyLib;
using Terraria.GameContent.ItemDropRules;
using Utils;

namespace DeterministicDrops.Patches;

[HarmonyPatch(typeof(MechBossSpawnersDropRule), nameof(MechBossSpawnersDropRule.TryDroppingItem))]
internal sealed class MechBossSpawnersDropRulePatch : Patch<Mod>
{
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
