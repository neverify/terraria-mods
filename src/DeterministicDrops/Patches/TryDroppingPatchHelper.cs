using System;
using System.Collections.Generic;
using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class TryDroppingPatchHelper
{
    public static bool HandleDrop<IItemDropRule>
    (DropAttemptInfo info, IItemDropRule __instance, Func<IItemDropRule, DropStateStore, IEnumerable<DropResult>> dropRuleProcessor, ref ItemDropAttemptResult __result)
    {
        if (!Mod.Instance.Config.EnableDeterministicDrops)
            return true;

        var drops = dropRuleProcessor(__instance, Mod.Instance.DropStateStore);

        bool dropped = false;

        foreach (var result in drops)
        {
            CommonCode.DropItemFromNPC(info.npc, result.ItemId, result.Amount);
            dropped = true;
        }

        __result = new ItemDropAttemptResult
        {
            State = dropped
                ? ItemDropAttemptResultState.Success
                : ItemDropAttemptResultState.FailedRandomRoll
        };

        return false;
    }
}
