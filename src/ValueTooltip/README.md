# Value Tooltip

Display the sell value of items in their tooltip even outside of shops.

Knowing the value of items is a very useful thing in Terraria. Especially in the early-game when inventory space is limited, knowing which items are worth picking up matters. Since this information is not available outside of shops, you have to manually look it up in the Wiki, which is slow and annoying.

This mod fixes the issue by adding a line to all items' tooltips that displays their sell value. Coins and items that do not have a sell value are excluded from this. Stacked items show the total value with the base value displayed in parentheses.

## Config

### Show Value Tooltips

Show the sell value of items in their tooltip.

## Development

### Harmony Patches

#### `Main.MouseText_DrawItemTooltip_GetLinesInfo()`

```cs
public static void MouseText_DrawItemTooltip_GetLinesInfo(Item item, ref int yoyoLogo, ref int researchLine, float oldKB, ref int numLines, string[] toolTipLine, Color[] lineColors)
```

This method is responsible for building the tooltip for items. The method is just a long list of conditions of all of the possible tooltip entries, so it is not shown here. The important part is the signature of the method.

The mod applies a postfix patch to this method to add the sell value of the item to the tooltip. The tooltips are stored in the `toolTipLine` string array, and the `numLines` variable keeps track of the number of lines in the tooltip. The value tooltip is simply placed at the end of the `toolTipLine` array, and the `numLines` variable is incremented by one.
