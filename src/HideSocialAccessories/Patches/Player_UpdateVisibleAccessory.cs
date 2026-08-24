using HarmonyLib;
using Terraria;

namespace HideSocialAccessories.Patches;

[HarmonyPatch(typeof(Player), "UpdateVisibleAccessory")]
internal static class UpdateVisibleAccessoryPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(Player __instance, int itemSlot) =>
        !(itemSlot is >= 13 and < 20 && __instance.hideVisibleAccessory[itemSlot - 10]);
}
