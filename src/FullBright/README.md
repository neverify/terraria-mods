# Fullbright

Force all tiles to render at a configured brightness.

This mod is a performant implementation of fullbright. The vanilla rendering engine can't normally handle rendering all the tiles at full brightness at 60fps. This is why most naive fullbright implementations lag the game considerably. This mod implements performance optimizations to minimize the impact of rendering more tiles than intended. You should be able to retain 60fps even on weaker hardware.

The mod also allows overriding the brightness of the map, so it will actually reflect what you see in-game.

The mod modifies the lighting engine of the "Color" lighting mode, so other lighting modes are unaffected.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Brightness Override

Enable the brightness override. Only works with the "color" lighting mode.

### Brightness

The brightness at which to render tiles at. Decimal number between `0` and `1` (e.g. `0.1` = 10%).

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

## Technical Overview

### Tile Lighting

The lighting engine obtains the colors of tiles from the `Graphics.Light.LightingEngine.GetColor()` method. It returns black for tiles that are outside the `_activeProcessedArea` rectangle, and otherwise reads their color from the `_activeLightMap` value. The `Graphics.Light.LightingEngine.ProcessScan()` method updates `_activeProcessedArea` as the camera moves.

The mod replaces the tile color lookup with the configured brightness. Because the `GetColor()` method does not distinguish between tiles that are outside `_activeProcessedArea` and just unlit ones (both are returned as black), simply overriding this color would result in the rendering engine rendering tiles outside the camera viewport, resulting in a performance decrease. This is why the mod runs that check again manually. The value of `_activeProcessedArea` is cached after each update, so the check does not require reflection on every tile query.

### Map Lighting

The map consists of individual tiles all with their own brightness value. The map is updated by the `Map.WorldMap.UpdateLighting()` method.

The mod replaces this update with the configured brightness. Because the vanilla method does not allow the brightness to be set to a value lower than the current one, it is completely replaced.

### Light Smoothing

The method `GameContent.Drawing.TileDrawing.DrawSingleTile_SlicedBlock()` handles drawing most tiles. It applies one of three levels of light smoothing depending on the brightness of the tile and some cutoff thresholds calculated by the method `GameContent.Drawing.TileDrawing.Draw()`. These thresholds are determined by the quality settings of the game.

Light smoothing is an expensive operation, which becomes useless with the mod enabled. Thus the mod disables it by enabling the built-in `DebugOptions.devLightTilesCheat` toggle. That toggle sets the cutoff thresholds so high that light smoothing is skipped for all tiles.
