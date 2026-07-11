# Value Tooltip

Display the sell value of items in their tooltip even outside of shops.

## Config

### Show Value Tooltips

Show the sell value of items in their tooltip.

## Development

### Harmony Patches

#### `Main.MouseText_DrawItemTooltip_GetLinesInfo()`

This method is responsible for building the tooltip for items. The mod applies a postfix patch onto this method to add the sell value of the item to the tooltip.

The sell value is formatted to include the counts of each coin. The sell value is multiplied with the stack size of the item to get the total sell value. If the stack is more than one item, the base sell value is displayed in parentheses at the end. The tooltip is colored based on the highest full coin.
