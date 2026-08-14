using System.Collections.Generic;
using HarmonyLib;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;

namespace SelectOres.Patches;

[HarmonyPatch(typeof(WorldGen), nameof(WorldGen.Reset))]
internal sealed class ResetPatch
{
    internal static void Prefix()
    {
        if (!Mod.Config.OverrideGeneration)
            return;

        SetOreTier(ref WorldGen.SavedOreTiers.Cobalt, Mod.Config.HardmodeTier1Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Mythril, Mod.Config.HardmodeTier2Ore);
        SetOreTier(ref WorldGen.SavedOreTiers.Adamantite, Mod.Config.HardmodeTier3Ore);
    }

    internal static void Postfix()
    {
        if (!Mod.Config.OverrideGeneration)
            return;

        GenVars.copper = s_oreNameToTileId[Mod.Config.Tier1Ore];
        GenVars.iron = s_oreNameToTileId[Mod.Config.Tier2Ore];
        GenVars.silver = s_oreNameToTileId[Mod.Config.Tier3Ore];
        GenVars.gold = s_oreNameToTileId[Mod.Config.Tier4Ore];

        GenVars.copperBar = s_oreNameToBarId[Mod.Config.Tier1Ore];
        GenVars.ironBar = s_oreNameToBarId[Mod.Config.Tier2Ore];
        GenVars.silverBar = s_oreNameToBarId[Mod.Config.Tier3Ore];
        GenVars.goldBar = s_oreNameToBarId[Mod.Config.Tier4Ore];

        WorldGen.SavedOreTiers.Copper = s_oreNameToTileId[Mod.Config.Tier1Ore];
        WorldGen.SavedOreTiers.Iron = s_oreNameToTileId[Mod.Config.Tier2Ore];
        WorldGen.SavedOreTiers.Silver = s_oreNameToTileId[Mod.Config.Tier3Ore];
        WorldGen.SavedOreTiers.Gold = s_oreNameToTileId[Mod.Config.Tier4Ore];
    }

    private static void SetOreTier(ref int savedTier, string oreName)
    {
        if (savedTier == -1)
            savedTier = s_oreNameToTileId[oreName];
    }

    private static readonly Dictionary<string, int> s_oreNameToTileId = new()
    {
        { "Copper", TileID.Copper },
        { "Iron", TileID.Iron },
        { "Silver", TileID.Silver },
        { "Gold", TileID.Gold },
        { "Tin", TileID.Tin },
        { "Lead", TileID.Lead },
        { "Tungsten", TileID.Tungsten },
        { "Platinum", TileID.Platinum },
        { "Cobalt", TileID.Cobalt },
        { "Mythril", TileID.Mythril },
        { "Adamantite", TileID.Adamantite },
        { "Palladium", TileID.Palladium },
        { "Orichalcum", TileID.Orichalcum },
        { "Titanium", TileID.Titanium },
    };

    private static readonly Dictionary<string, int> s_oreNameToBarId = new()
    {
        { "Copper", ItemID.CopperBar },
        { "Iron", ItemID.IronBar },
        { "Silver", ItemID.SilverBar },
        { "Gold", ItemID.GoldBar },
        { "Tin", ItemID.TinBar },
        { "Lead", ItemID.LeadBar },
        { "Tungsten", ItemID.TungstenBar },
        { "Platinum", ItemID.PlatinumBar },
    };
}
