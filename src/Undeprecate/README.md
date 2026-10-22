# Undeprecate

Prevent Terraria from removing deprecated items.

There are many items in Terraria that are marked as deprecated in the code. These items are unobtainable within the game, and Terraria scans for and removes these items if players obtain them using an inventory editor or similar tool.

This mod prevents Terraria from removing these items completely. Your items are safe as long as other players without the mod don't use them.

The mod doesn't provide a way to obtain these items. I recommend using the [Item Spawner](https://www.nexusmods.com/terraria/mods/141) mod by Inidar. It features a simple GUI to add any of these items into your inventory.

There is also a journey mode player save file available for download, with all items researched and all of the deprecated items in the inventory, if you prefer that.

You can also use an inventory editor such as [Terrasavr](https://yal.cc/r/terrasavr/) to add the items. There is an inventory file available for Terrasavr for quickly adding all of these items. To use that file, open the Inventory tab in Terrasavr, then press the "Load" button and select the file.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Technical Overview

The arrays `ID.ItemID.Sets.Deprecated` and `ID.ItemID.Sets.ItemsThatShouldNotBeInInventory` are used to scan for items that the game will remove from the world.

The mod clears both arrays during initialization. The existing array instances are cleared rather than replaced so any vanilla code that already holds a reference to them observes the change.
