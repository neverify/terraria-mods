# Better Traveling Merchant

Increase the spawn rate of the Traveling Merchant and force certain items to always be sold.

The game rolls a 1 in 108000 chance each tick to attempt to spawn the Traveling Merchant. This results in the spawn rate following a binomial distribution, which means it is possible to go arbitrarily long without a single spawn. The logic for choosing which items to sell is also unnecessarily complicated and results in needing dozens of spawns on average to obtain all of the important items.

This mod alleviates these frustrations by increasing the spawn rate of the Traveling Merchant by a configurable amount and forcing the most useful items to always be sold.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Spawn Chance Multiplier

Multiply the chance of the traveling merchant spawning each tick. Decimal number between `1.0` and `100.0` (e.g. `2.0` = 2x).

### Additional Items

Make the Traveling Merchant always sell the items configured below.

### Hand of Creation Ingredients

Always sell the Hand of Creation ingredients.

These include the Brick Layer, Extendo Grip, Paint Sprayer and Portable Cement Mixer.

### Shellphone Ingredients

Always sell the Shellphone ingredients.

These include the DPS Meter, Lifeform Analyzer and Stopwatch.

## Technical Overview

### Spawn Chance Multiplier

The method `WorldGen.SpawnTravelNPC()` spawns the Traveling Merchant. `Main.UpdateTime()` rolls a 1/108000 chance each tick if the required conditions are met (i.e. time and NPC count), in which it calls the `SpawnTravelNPC()` method.

The mod patches the `UpdateTime()` method to roll the chance again with configurable odds. However, adding a second roll introduces a new distribution, resulting in a small error between the configured multiplier and true probability. This error is very small though (<1%), so it is not relevant.

### Additional Items

The method `Chest.SetupTravelShop()` selects the items sold by the Traveling Merchant. The method appends the selected items' IDs to the `Main.travelShop` array, which defines the items sold in the shop.

The mod patches this method to add the configured items to the end of the array. The patch first identifies the first open slot by looking for the first occurrence of `0` in the array. It then loops through the configured items and adds them to sequential slots.
