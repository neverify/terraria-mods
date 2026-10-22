# No Damage Variance

Disable the ±15% damage variance for all sources of damage.

In vanilla Terraria, almost all sources of damage are subject to a random variance of ±15%. This includes, but is not limited to, player damage to NPCs, NPC damage to players and environmental damage to both players and NPCs.

This mod disables that variance completely. This is useful for testing different damage setups, as the DPS meter will fluctuate much less. Additionally, this allows fine-tuning damage to for example always one-shot certain NPCs.

The mod is built for **Terraria 1.4.5.8** and requires **TerrariaModder 0.4.1**. The mod should work in multiplayer.

## Configuration

### Disable Damage Variance

Disable the ±15% damage variance for all sources of damage.

## Technical Overview

Damage variance for all sources of damage is applied with the method `Main.DamageVar()`.

There exists a debug option `Testing.DebugOptions.NoDamageVar`, which bypasses the method entirely. The mod simply toggles this option according to the config.
