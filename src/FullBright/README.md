# Fullbright

Force all tiles to render at a configured brightness.

This mod is a performant implementation of fullbright. The mod completely disables the vanilla lighting engine and implements several performance optimizations.

The mod overrides the lighting of the map, making it reflect what you see in-game. There are also commands for revealing and hiding the entire map.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Brightness Override

Enable the brightness override.

### Brightness

The brightness to render tiles at. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

### Lighting Padding

How much extra space to light around the viewport. Integer between `0` and `50`.

Lower values improve performance, but results in the edges of the viewport flashing black when moving fast enough. The default value of `8` should be fine for most cases. Increase the value if you run into side-effects.

### Update Map Lighting

Whether to update map lighting.

With this option toggled off, map lighting is not updated at all. If you don't want an existing map being updated, or you have used the /map command to reveal the entire map, this option should be disabled.

### Map Brightness

The brightness of the map. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

## Keybinds

### Toggle Brightness Override

Toggle the brightness override. Default: N/A.

### Increase Brightness

Increase fullbright brightness. Default: N/A.

### Decrease Brightness

Decrease fullbright brightness. Default: N/A.

## Commands

### `/map`

Set the brightness of the entire map to the value of the config option "Map Brightness".

### `/clearmap`

Reset the map completely.

## Technical Overview

### Tile Lighting

Terraria has two separate lighting engines that implement the `Graphics.Light.ILightingEngine` interface: `LightingEngine` (color) and `LegacyLighting` (white, retro, trippy). The `Lighting` class holds an instance of both of the engines, handles swapping between them, and calls the methods of the active engine.

The mod implements a custom lighting engine `FullBrightLightingEngine`, which acts as a stub. The `ILightingEngine` interface has only one method with a return value: `GetColor()`, which returns the lighting color of a queried tile. However, the rendering engine queries tile colors in an area way larger than the viewport, but only renders tiles which are not black. That is why the vanilla lighting engine keeps track of a smaller area around the viewport, outside of which all tiles are considered black. This area is updated only every four lighting updates though, which results in it lagging behind the player position. That is why it is inflated past the viewport size, but that causes the rendering engine to render more tiles, reducing performance. The custom lighting engine instead updates this area on every call to the interface method `ProcessArea()`, which allows reducing the padding, improving performance considerably.

The active lighting engine is stored in the `_activeEngine` field, which is updated by the setter of the `Mode` property, which stores the selected lighting mode. The mod patches the setter, forcing `_activeEngine` to `FullBrightLightingEngine`, and `Mode` to `LightMode.Color`, in order for the `Lighting` class to act as if the "Color" lighting engine was selected.

### Render Scheduling

The method `Main.RenderToTargets()` draws different render targets in a four-frame cycle controlled by the `Main.renderCount` field, which is controlled by the lighting engine. However, the lighting engine only updates the lighting area every four cycles, which is why the lighting area is inflated by 28 tiles, reducing performance.

The mod replaces this method with a load-balanced version. Instead of the render targets being grouped by category, they are grouped by performance. This reduces the longest render pass time and shrinks the render cycle to 3 frames. Furthermore the patched method updates the lighting area before each pass instead of after it, which allows shrinking the lighting padding a bit more, improving performance even more.

The vanilla lighting engine is also responsible for updating the minimap, and running `Main.UpdateSceneMetrics()`. These are moved to the patched `Main.RenderToTargets()` method, which runs them in the third cycle, which has the most overhead. The minimap update is implemented as a simple nested loop, which calls the patched `Map.WorldMap.UpdateLighting()` method for each tile in the lighting area around the player.

### Light Smoothing

The method `GameContent.Drawing.TileDrawing.DrawSingleTile_SlicedBlock()` handles drawing most tiles. It applies one of three levels of light smoothing depending on the brightness of the tile and some cutoff thresholds calculated by the method `GameContent.Drawing.TileDrawing.Draw()`. These thresholds are determined by the quality settings of the game.

Light smoothing is an expensive operation, which becomes useless with the mod enabled. Thus the mod disables it by enabling the built-in `DebugOptions.devLightTilesCheat` toggle. That toggle sets the cutoff thresholds so high that light smoothing is skipped for all tiles.

### Map Lighting

The map consists of individual tiles all with their own brightness value. The map is updated by the `Map.WorldMap.UpdateLighting()` method.

The mod replaces this update with the configured brightness. Because the vanilla method does not allow the brightness to be set to a value lower than the current one, it is completely replaced.

### Map Commands

The `Testing.ChatCommands` namespace contains commands used in the development of Terraria. The class `Testing.ChatCommands.ToolkitDebugCommands` contains the definitions of all of the commands, including `/map` and `/clearmap`, which the mod uses. These commands are disabled by default, and controlled by the `DebugOptions.enableDebugCommands` field. However, simply setting the field to `true` is not sufficient, as that would unintentionally enable all the other debug commands as well. Thus the mod has to enable only those two commands.

The method `Main.DoUpdate_HandleChat()` handles chat input and messages. The method tries to execute each chat message as a debug command with the `Process()` method of the `Testing.ChatCommands.DebugCommandProcessor` class instance stored in the field `UI.Chat.ChatManager.DebugCommands`, only processing the message normally if that fails. The `Process()` method checks the `enableDebugCommands` field, and only runs if it is `true`. It then tries to find the command in its `_commands` dictionary. If the command is found and its conditions are met, it is ran.

The mod patches the `Process()` method to set the `enableDebugCommands` field to `true` for the duration of the method, if the `CommandName` property of the `DebugMessage` argument is either `map` or `clearmap`. This effectively allows those commands to bypass the usual requirement.

The `_commands` dictionary is initialized in the `DebugCommandProcessor` constructor with its method `AddAttributeCommandsFromType()`, but only if the `enableDebugCommands` field is `true`.

The mod patches the constructor to set the `enableDebugCommands` field to `true` for the duration of the constructor. This forces the commands to be initialized.

Internally the `/map` command uses the `Testing.UnlockMapSection()` method, which calls the `Map.WorldMap.UpdateLighting()` method with `byte.MaxValue` as the brightness argument. However, the mod already patches this method with an implementation that ignores that parameter, instead setting the brightness of the tile to the configured value. Thus the `/map` command ends up setting the brightness of all tiles to the configured value, instead of maximum brightness.
