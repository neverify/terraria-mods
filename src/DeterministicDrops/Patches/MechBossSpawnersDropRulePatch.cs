using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class MechBossSpawnersDropRulePatch
{
    public static bool TryDroppingItemPrefix(DropAttemptInfo info, MechBossSpawnersDropRule __instance, ref ItemDropAttemptResult __result)
    => TryDroppingPatchHelper.HandleDrop(info, __instance, DropProcessor.ProcessMechBossSpawnersDropRule, ref __result);
}
