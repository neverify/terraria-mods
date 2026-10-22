# Rare Drop Notification

Show a configurable chat message when a rare item is dropped.

Terraria has a lot of item drops, so it is easy to miss the rare ones. This mod makes sure you never miss a drop you care about again.

The mod only shows notifications for drops with a simple 1/x chance. Drops that always occur and pick from a list of alternatives with equal odds, such as mimics and many boss drops, are not notified.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Show Notifications

Show a chat message when a rare item is dropped.

### Notification Threshold

The minimum drop chance for the chat message to be sent. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

### Notification Format

Placeholders: `{itemId}`, `{itemName}`, `{chance}`.

### Low Rarity Notification Color

Color of low rarity drops. Hex color code (`#RRGGBB`).

### High Rarity Notification Color

Color of high rarity drops. Hex color code (`#RRGGBB`).

## Technical Overview

### Detecting Drops

NPC drops implement the `GameContent.ItemDropRules.IItemDropRule` interface. The implementation used by simple 1/x drops is `GameContent.ItemDropRules.CommonDrop`. The method `GameContent.ItemDropRules.ItemDropResolver.ResolveRule()` evaluates all drop rules, returning the result of the drop.

The mod listens for successful drop results of `CommonDrop` drops from the `ResolveRule()` method, passing them to the notification handler.

### Displaying Notifications

The `DropNotification` class handles displaying the chat messages.

The message is formatted with the shared `PlaceholderFormatter` class. The hex colors are parsed manually and converted to `Color`-objects, which are then linearly interpolated to obtain the final color based on the drop chance.

## Credits

This mod is a partial recreation of the [tModLoader mod](https://steamcommunity.com/sharedfiles/filedetails/?id=3400340796) with the same name by Pearlie.
