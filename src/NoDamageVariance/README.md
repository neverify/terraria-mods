# No Damage Variance

Disable the ±15% damage variance for all sources of damage.

In vanilla Terraria, almost all sources of damage are subject to a random variance of ±15%. This includes, but is not limited to, player damage to NPCs, NPC damage to players and environmental damage to both players and NPCs.

This mod disables that variance completely. This is useful for testing different damage setups, as the DPS meter will fluctuate much less. Additionally, this allows fine-tuning damage to for example always one-shot certain NPCs.

The mod should work in multiplayer.

## Configuration

### Disable Damage Variance

Disable the ±15% damage variance for all sources of damage.

## Development

This mod is extremely simple. Damage variance for all sources of damage is calculated using a single method `Terraria.Main.DamageVar()`. Thus applying a simple harmony prefix patch to conditionally skip the method (return the original value) would be all that is needed.

However, there is an even simpler and more idiomatic way to achieve this. The `Terraria.Testing.DebugOptions` class contains a selection of toggles used by the developers for testing. One of these toggles is `NoDamageVar`, which does exactly what it states. The `DamageVar()` method has an early return for this toggle baked-in. The properties in `DebugOptions` are `public`, so no reflection is needed to set them.

The mod simply sets this value according to the config in the `Initialize()` method and when the config is changed.
