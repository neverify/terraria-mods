using System;
using Terraria.ID;
using Utils;

namespace Undeprecate;

public class Mod : ModBase<Mod, Config>
{
    public override string Id => "undeprecate";
    public override string Name => "Undeprecate";
    public override string Version => "1.0.1";

    protected override void Initialize()
    {
        Array.Clear(ItemID.Sets.Deprecated, 0, ItemID.Sets.Deprecated.Length);
        Array.Clear(
            ItemID.Sets.ItemsThatShouldNotBeInInventory,
            0,
            ItemID.Sets.ItemsThatShouldNotBeInInventory.Length
        );
    }
}
