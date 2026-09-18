using System;
using Terraria.ID;
using Utils;

namespace Undeprecate;

public class Mod : ModBase<Mod, Config>
{
    protected override void OnInitialize()
    {
        Array.Clear(ItemID.Sets.Deprecated, 0, ItemID.Sets.Deprecated.Length);
        Array.Clear(
            ItemID.Sets.ItemsThatShouldNotBeInInventory,
            0,
            ItemID.Sets.ItemsThatShouldNotBeInInventory.Length
        );
    }
}
