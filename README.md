![AnyAngle - Build at any angle](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/anyangle-banner.png)

<br>

**Build at any angle.**

Move and rotate the build preview on every axis before you place it. Sink a piece into the ground, push it a few centimetres, tilt it, and it is built exactly where the preview is.

> **Install it on your client only.** The server and other players do not need it, and they see your pieces normally. Valheim 1.0 Â· BepInExPack 5.4.2350+ Â· Console players cannot load the mod.

[Bug reports](https://github.com/morda0511/AnyAngle/issues) Â· [Discord](https://discord.gg/sHZzWFBpRm) Â· [GitHub](https://github.com/morda0511/AnyAngle)

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">At a glance</h2>

- **Move** the piece up, down, forward, back, left and right in fine steps
- **Tilt, roll and turn** it on all three axes (vanilla only turns in 22.5 degree steps)
- **Place it anyway:** pieces can be built even when Valheim marks the preview invalid because of clipping or slope
- **Snaps to your nudged pieces** the same way it snaps to normal ones
- **Copy** the offset and rotation of a piece you look at
- **Key box** in the bottom right shows all keys, the step size and the current offset and rotation

Ward zones and no-build zones are always respected.

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">Keys</h2>

All keys can be changed in `BepInEx/config/com.morda.anyangle.cfg`. Hold a key to repeat it.

| Action | Key |
|---|---|
| Move forward / back | Up / Down arrow |
| Move left / right | Left / Right arrow |
| Move up / down | Page Up / Page Down |
| Tilt | Numpad 2 / Numpad 8 |
| Roll | Numpad 4 / Numpad 6 |
| Extra turn | Numpad 7 / Numpad 9 |
| Step size (fine, medium, coarse, large) | Numpad + |
| Reset | Numpad 5 |
| Copy offset and rotation from a piece | Alt + C |

Steps: 1 cm / 1 degree, 5 cm / 5 degrees, 25 cm / 15 degrees, 1 m / 45 degrees. The mouse wheel still turns the piece like in vanilla.

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">Settings</h2>

- **Enabled:** turns the mod on or off
- **IgnoreInvalid** (on by default): while a nudge is active, a preview that is marked invalid can still be placed
- **ResetAfterPlace** (off by default): resets offset and rotation after each piece. Off = the same offset stays for the next pieces
- **MaxOffset** (3 m): how far a piece can be moved from its normal spot
- **ShowHud:** shows the key box

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">Good to know</h2>

- This is an early version. Pieces you nudge store their offset as an extra value in the world data. The game and other mods ignore it
- Mods that also change the build preview or snapping (for example PlanBuild or snap point mods) may conflict
- A nudged piece that hangs in the air can still collapse if it has no support

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">More mods by Morda</h2>

| Mod | What it does |
|:---|:---|
| **[Store & Craft](https://thunderstore.io/c/valheim/p/Morda/StoreAndCraft/)** | Auto-store drops, dump your inventory, and craft or build from nearby chests |
| **[WorkbenchesPlus](https://thunderstore.io/c/valheim/p/Morda/WorkbenchesPlus/)** | Sort and filter crafting menus: craftable first, categories, armor and weapon sets by biome tier |
| **[Forager's Garden](https://thunderstore.io/c/valheim/p/Morda/ForagersGarden/)** | Berry bushes, wild herbs and mushrooms with the Cultivator, plus Ice, Royal Jelly and Crystal beehives |
| **[Morda's Gearhold](https://thunderstore.io/c/valheim/p/Morda/MordasGearhold/)** | Three switchable equipment sets, shared inventory, 8 hotkey quickslots, ammo slots, trinkets, transmog looks and armour dyes |
| **[Valheim Mannequins](https://thunderstore.io/c/valheim/p/Morda/ValheimMannequins/)** | Player-shaped mannequins: dress them with real gear, pose and name them, keep several gear sets |
| **[ServerRates](https://thunderstore.io/c/valheim/p/Morda/ServerRates/)** | Server-side rates for dedicated servers (PC and console), live apply, presets. Joining players need no install |
| **[SmoothBuildRepair](https://thunderstore.io/c/valheim/p/Morda/SmoothBuildRepair/)** | Automatically repairs nearby buildings a few pieces at a time, no lag |

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/divider-norse.png)

<h2 align="center">Install</h2>

1. Install [BepInExPack Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)
2. Put `AnyAngle.dll` in `BepInEx/plugins/AnyAngle/` (or use Thunderstore / r2modman)
3. Servers do not need the mod

![AnyAngle](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/anyangle-footer.png)
