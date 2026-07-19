using DeterministicDrops.DropSystem;
using Terraria.GameContent.ItemDropRules;

namespace DeterministicDrops.Patches;

internal static class TryDroppingItemPatch
{
    internal static bool Prefix(DropAttemptInfo info, CommonDrop __instance, ref ItemDropAttemptResult __result)
    {
        if (!Mod.Instance.Config.EnableDeterministicDrops)
            return true;

        var dropStateStore = Mod.Instance.DropStateStore;
        int dropAmount = DropScheduler.ProcessKill(__instance, dropStateStore);
        bool dropped = dropAmount > 0;

        if (dropped)
            CommonCode.DropItemFromNPC(info.npc, __instance.itemId, dropAmount);

        __result = new ItemDropAttemptResult
        {
            State = dropped
            ? ItemDropAttemptResultState.Success
            : ItemDropAttemptResultState.FailedRandomRoll
        };

        return false;
    }
}
