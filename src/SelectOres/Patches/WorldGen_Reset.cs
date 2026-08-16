using System.Collections.Generic;
using HarmonyLib;
using Terraria;
using Terraria.ID;
using Terraria.WorldBuilding;
using Utils;

namespace SelectOres.Patches;

[HarmonyPatch(typeof(WorldGen), nameof(WorldGen.Reset))]
internal sealed class ResetPatch : Patch<Mod>
{
    private static void Postfix()
    {
        if (!Mod.Instance.Config.OverrideGeneration)
            return;

        GenVars.copper = s_oreNameToTileId[Mod.Instance.Config.Tier1Ore];
        GenVars.iron = s_oreNameToTileId[Mod.Instance.Config.Tier2Ore];
        GenVars.silver = s_oreNameToTileId[Mod.Instance.Config.Tier3Ore];
        GenVars.gold = s_oreNameToTileId[Mod.Instance.Config.Tier4Ore];

        GenVars.copperBar = s_oreNameToBarId[Mod.Instance.Config.Tier1Ore];
        GenVars.ironBar = s_oreNameToBarId[Mod.Instance.Config.Tier2Ore];
        GenVars.silverBar = s_oreNameToBarId[Mod.Instance.Config.Tier3Ore];
        GenVars.goldBar = s_oreNameToBarId[Mod.Instance.Config.Tier4Ore];

        WorldGen.SavedOreTiers.Copper = s_oreNameToTileId[Mod.Instance.Config.Tier1Ore];
        WorldGen.SavedOreTiers.Iron = s_oreNameToTileId[Mod.Instance.Config.Tier2Ore];
        WorldGen.SavedOreTiers.Silver = s_oreNameToTileId[Mod.Instance.Config.Tier3Ore];
        WorldGen.SavedOreTiers.Gold = s_oreNameToTileId[Mod.Instance.Config.Tier4Ore];
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
