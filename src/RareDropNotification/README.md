# Rare Drop Notification

Show a configurable chat message when a rare item is dropped.

The mod only shows notifications for drops with a 1/x chance. Drops that always occur and pick from a list of alternatives with equal odds, such as many boss drops and mimics, are not notified.

## Configuration

### Show Notifications

Show a chat message when a rare item is dropped.

### Notification Threshold

The minimum drop chance for the chat message to be sent. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

### Drop Notification Format

The format of the chat message. The following placeholders are available: `{itemId}`, `{itemName}`, `{chance}`. This config option can't be edited in-game due to framework limitations.

### Low Rarity Notification Color

The gradient edge for low rarity drops. Hex color code. This config option can't be edited in-game due to framework limitations.

### High Rarity Notification Color

The gradient edge for high rarity drops. Hex color code. This config option can't be edited in-game due to framework limitations.

## Development

### Harmony patches

#### `Terraria.GameContent.ItemDropRules.ItemDropResolver.ResolveRule()`

```cs
private ItemDropAttemptResult ResolveRule(IItemDropRule rule, DropAttemptInfo info)
{
    if (!rule.CanDrop(info))
    {
        ItemDropAttemptResult itemDropAttemptResult = new ItemDropAttemptResult
        {
            State = ItemDropAttemptResultState.DoesntFillConditions
        };
        this.ResolveRuleChains(rule, info, itemDropAttemptResult);
        return itemDropAttemptResult;
    }
    INestedItemDropRule nestedItemDropRule = rule as INestedItemDropRule;
    ItemDropAttemptResult itemDropAttemptResult2;
    if (nestedItemDropRule != null)
    {
        itemDropAttemptResult2 = nestedItemDropRule.TryDroppingItem(info, new ItemDropRuleResolveAction(this.ResolveRule));
    }
    else
    {
        itemDropAttemptResult2 = rule.TryDroppingItem(info);
    }
    this.ResolveRuleChains(rule, info, itemDropAttemptResult2);
    return itemDropAttemptResult2;
}
```

This method resolves an item drop rule and returns the result of the drop attempt. The mod applies a postfix patch onto this method in order to obtain the result of the drop attempt. The result is then passed on to the notification handler.

### Other features

#### `DropNotification`

The `DropNotification` class handles the logic for displaying the chat messages. The message format is parsed with Regex. The hex colors are parsed manually and converted to `Color`-objects, which are then linearly interpolated to obtain the final color based on the drop chance.

## Credits

This mod is a partial recreation of the [tModLoader mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3400340796) with the same name by Pearlie.
