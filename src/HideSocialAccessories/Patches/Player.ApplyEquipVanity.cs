using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using Terraria;

namespace HideSocialAccessories.Patches;

[SuppressMessage("Style", "IDE0051: Remove unused private members")]
[HarmonyPatch(typeof(Player), "ApplyEquipVanity")]
internal static class ApplyEquipVanityPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static bool Prefix(Player __instance, int itemSlot) =>
        !__instance.hideVisibleAccessory[itemSlot - 10];
}
