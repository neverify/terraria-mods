# Undeprecate

Prevent Terraria from removing deprecated items.

There are many items in Terraria that are marked as deprecated in the code. These items are unobtainable within the game, and Terraria scans for and removes these items if players obtain them using an inventory editor or similar tool.

The mod prevents Terraria from removing these items completely. Your items are safe as long as other players without the mod don't use them.

The mod should work in multiplayer.

## Development

This mod is very simple. It clears the `ItemID.Sets.Deprecated` and `ItemID.Sets.ItemsThatShouldNotBeInInventory` arrays during initialization. These arrays are used in the many methods that Terraria uses to scan for deprecated items. The arrays are cleared instead of replaced with new arrays in case the array instances are cached somewhere.
