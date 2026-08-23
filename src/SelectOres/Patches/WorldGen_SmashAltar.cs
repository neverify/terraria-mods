using System.Collections.Generic;
using HarmonyLib;
using Terraria;
using Terraria.ID;

namespace SelectOres.Patches;

[HarmonyPatch(typeof(WorldGen), nameof(WorldGen.SmashAltar))]
internal static class SmashAltarPatch
{
    private static bool Prepare() => Mod.Instance is not null;

    private static void Prefix()
    {
        if (!Mod.Instance.Config.OverrideGeneration)
            return;

        SetOreTier(ref WorldGen.SavedOreTiers.Cobalt, Mod.Instance.Config.HardmodeTier1Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Mythril, Mod.Instance.Config.HardmodeTier2Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Adamantite, Mod.Instance.Config.HardmodeTier3Ore);
    }

    private static void SetOreTier(ref int savedTier, string oreName)
    {
        if (savedTier == -1)
            savedTier = s_oreNameToTileId[oreName];
    }

    private static readonly Dictionary<string, int> s_oreNameToTileId = new()
    {
        { "Cobalt", TileID.Cobalt },
        { "Mythril", TileID.Mythril },
        { "Adamantite", TileID.Adamantite },
        { "Palladium", TileID.Palladium },
        { "Orichalcum", TileID.Orichalcum },
        { "Titanium", TileID.Titanium },
    };
}
