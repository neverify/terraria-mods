# Better Traveling Merchant

Increase the spawn rate of the Traveling Merchant and force certain items to always be sold.

The game rolls a 1 in 108000 chance each tick to attempt to spawn the Traveling Merchant. This results in the spawn rate following a binomial distribution, which means it is possible to go arbitrarily long without a single spawn. The logic for choosing which items to sell is also unnecessarily complicated, and results in dozens of spawns being required to obtain all of the important items.

This mod alleviates these frustrations by increasing the spawn rate of the Traveling Merchant by a configurable amount and forcing the most useful items to always be sold.

## Configuration

### Spawn Chance Multiplier

Multiply the chance of the traveling merchant spawning each tick. Decimal number between `1.0` and `100.0`.

### Additional Items

Make the Traveling Merchant always sell the items configured below.

### Hand of Creation Ingredients

Always sell the Hand of Creation ingredients.

These include the Brick Layer, Extendo Grip, Paint Sprayer and Portable Cement Mixer.

### Shellphone Ingredients

Always sell the Shellphone ingredients.

These include the DPS Meter, Lifeform Analyzer and Stopwatch.

## Development

### Harmony Patches

#### `Terraria.Main.UpdateTime()`

```cs
private static void UpdateTime()
{
    // ...

    else if (!IsFastForwardingTime() && dayTime && time < 27000.0)
    {
        // ...

        int num7 = (int)(27000.0 / (double)num6);
        num7 *= 4;
        if (rand.Next(num7) == 0)
        {
            // ...

            WorldGen.SpawnTravelNPC();

            // ...
        }
    }
}
```

This method handles spawning the Traveling Merchant. The spawn is attempted with a 1 in 108000 chance each tick.

The mod applies a postfix patch onto this method to roll the chance again with higher odds. It is not possible to easily prevent the original chance from being rolled. Adding a second roll introduces a new distribution. However, since the sum of two binomial distributions does not equal the distribution produced by increasing the original chance, there is a slight error between the configured multiplier and actual probability. However, since the base chance is so low, this error is negligible. The error could be accounted for, but I decided against it to keep the logic of the mod simple.

#### `Terraria.Chest.SetupTravelShop()`

This method handles selecting the items sold by the Traveling Merchant. The implementation is quite long and frankly irrelevant, so it is not included here. The important part is that the method modifies the `Main.travelShop` array. The array contains the ids of all the sold items in order.

The mod applies a postfix patch onto this method to add the configured items into the shop. The patch first identifies the first open slot by looking for the first occurence of `0` in the array. Then it loops through the configured items and adds them to the first empty slot.
