# Value Tooltip

Display the sell value of items in their tooltip even outside of shops.

Knowing the value of items is a very useful thing in Terraria. Especially in the early-game when inventory space is limited, knowing which items are worth picking up matters. Since this information is not available outside of shops, you have to manually look it up in the Wiki, which is slow and annoying.

This mod fixes the issue by adding a line to all items' tooltips that displays their sell value. Coins and items that do not have a sell value are excluded from this. Stacked items show the total value with the base value displayed in parentheses. The tooltip can be customized.

The mod also displays the combined sell value of the items in the inventory and in an open chest. These displays are similar to the "Savings" display when interacting with NPC shops. Both displays can be hidden and independently positioned.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Show Value Tooltips

Show the sell value of items in their tooltip.

### Singular Item Tooltip Format

Placeholders: `{value}`.

### Stacked Item Tooltip Format

Placeholders: `{value}`, `{totalValue}`.

### Show Inventory Value

Show the combined value of all items in the inventory.

### Inventory Value X Position

The X position of the inventory value display.

### Inventory Value Y Position

The Y position of the inventory value display.

### Show Chest Value

Show the combined value of all items in an open chest.

### Chest Value X Position

The X position of the chest value display.

### Chest Value Y Position

The Y position of the chest value display.

## Technical Explanation

### Creating the Tooltip

The vanilla method responsible for building the tooltip of items is `Main.MouseText_DrawItemTooltip_GetLinesInfo()`. The method collects each line of the tooltip into an array `toolTipLine`. This allows the mod to simply apply a postfix patch to this method to add the sell value of the item as the last line of the tooltip.

The value string is constructed manually from the `Item.value` property. The tooltip is formatted according to the configured template with the shared `PlaceholderFormatter` class.

### Drawing the Value Displays

In vanilla, the "Savings" display is drawn with the `Terraria.UI.ItemSlot.DrawSavings()` method. This method internally uses `Terraria.UI.ItemSlot.DrawMoney()` to draw the actual values. The mod applies a postfix patch to the `Main.DrawInventory()` method, which handles drawing the inventory, to draw the value displays using the `DrawMoney()` method.

The inventory value is calculated from the player's inventory slots, while the chest value is calculated from the items in the open chest.
