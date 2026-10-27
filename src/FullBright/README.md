# Fullbright

Force all tiles to render at a configured brightness.

This mod is a performant implementation of fullbright. The vanilla rendering engine can't normally handle rendering all the tiles at full brightness at 60fps. This is why most naive fullbright implementations lag the game considerably. This mod implements performance optimizations to minimize the impact of rendering more tiles than intended. You should be able to retain 60fps even on weaker hardware.

The mod also allows overriding the brightness of the map, so it will actually reflect what you see in-game. There are also commands for manipulating the brightness of the entire map.

The mod modifies the lighting engine of the "Color" lighting mode, so other lighting modes are unaffected by the "Brightness Override" config option.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Brightness Override

Enable the brightness override. Only works with the "color" lighting mode.

### Brightness

The brightness to render tiles at. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

### Lighting Area Size Offset

An offset to decrease the size of the lighting area by.

A value of `0` is equivalent to vanilla. Lower values improve performance, but can result in black bars appearing at the edges of the screen when moving fast enough. The default value of `-20` is a good starting point, but it can be increased if you run into side-effects.

### Disable Light Smoothing

Disable vanilla light smoothing.

Recommended with brightness override to improve performance. This option has no visual impact with brightness override on, since all tiles are the same brightness.

### Map Lighting Override

Override the brightness of map lighting.

### Map Lighting Brightness

The brightness to set map tiles to. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

## Keybinds

### Toggle Brightness Override

Toggle the brightness override. Default: N/A.

### Increase Brightness

Increase fullbright brightness. Default: N/A.

### Decrease Brightness

Decrease fullbright brightness. Default: N/A.

## Commands

### `/map`

Set the brightness of all map tiles to the value of the config option "Map Lighting Brightness".

### `/clearmap`

Reset the map completely.

## Technical Overview

### Tile Lighting

Terraria has two separate lighting engines that implement the `Graphics.Light.ILightingEngine` interface: `LightingEngine` (color) and `LegacyLighting` (white, retro, trippy). The `Lighting` class holds an instance of both of the engines, handles swapping between them, and calls the methods of the active engine. The mod patches the `LightingEngine` (color) class.

The interface method used to obtain tile colors is `ILightingEngine.GetColor()`. Its implementation in the `LightingEngine` class first checks if the given coordinates are inside the `_activeProcessedArea` rectangle. If they are not, it returns black (`Vector3.Zero`). Otherwise the method reads the color from the `_activeLightMap` value. The `_activeProcessedArea` rectangle is updated by the `Present()` method, which swaps it with the `_workingProcessedArea` rectangle. That value is updated in the `ProcessScan()` method, which itself is invoked by the interface method `ProcessArea()`. However, `ProcessArea()` is only called by the engine every second lighting update, resulting in the `_workingProcessedArea` rectangle, and subsequently the `_activeProcessedArea` rectangle lagging behind the camera position. To combat this, the lighting engine inflates the rectangles by `28` tiles in each direction. However, this results in lower performance, as the rendering engine determines which tiles to render by their color, causing it to potentially render off-screen tiles.

The mod forgoes this update logic, and instead directly captures the `area` parameter of the `ProcessArea()` method. The area is inflated by `28` tiles by default, but then deflated by the value of the "Lighting Area Size Offset" config option, to optionally reduce the padding. As the `ProcessArea()` method isn't called every frame, the lighting area still lags behind by a few frames, so some padding is required. The amount of padding depends on the movement speed of the camera, and `8` seems to be enough for normal gameplay. Thus `-20` is the default offset.

The mod patches the `GetColor()` method with an implementation that checks the queried tile against the cached `Area` property. If the tile is inside the area a brightness value according to the "Brightness" config option is returned, otherwise black is returned.

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
