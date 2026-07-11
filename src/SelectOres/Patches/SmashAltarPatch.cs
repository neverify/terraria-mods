using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace SelectOres.Patches;

internal sealed class SmashAltarPatch
{
    internal static void Prefix()
    {
        if (!Mod.Config.OverrideGeneration)
            return;

        SetOreTier(ref WorldGen.SavedOreTiers.Cobalt, Mod.Config.HardmodeTier1Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Mythril, Mod.Config.HardmodeTier2Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Adamantite, Mod.Config.HardmodeTier3Ore);
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
