![Any Angle - Build at any angle](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/anyangle-banner.png)

<br>

**Build at any angle.**

Move and rotate the build preview on every axis before you place it. Sink a piece into the ground, push it a few centimetres, tilt it, and it is built exactly where the preview is. Everything runs on the mouse wheel and a few modifier keys, a gizmo shows which axis you are on.

> **Install it on your client only.** The server and other players do not need it, and they see your pieces normally. Valheim 1.0 | BepInExPack 5.4.2350+ | Console players cannot load the mod.

[Bug reports](https://github.com/morda0511/AnyAngle/issues) | [Discord](https://discord.gg/sHZzWFBpRm) | [GitHub](https://github.com/morda0511/AnyAngle)

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-at-a-glance.png)

| ![Move](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/glance-move-192.png) | ![Rotate](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/glance-rotate-192.png) | ![Copy](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/glance-copy-192.png) |
|:---:|:---:|:---:|
| **Move**<br>Up, down, forward, back, left and right in fine steps | **Rotate**<br>Tilt, roll and turn on all three axes | **Copy**<br>Take over the offset and rotation of a piece you look at |

- **Place it anyway:** pieces can be built even when Valheim marks the preview invalid because of clipping or slope
- **Snaps to your nudged pieces** the same way it snaps to normal ones, on the sides, the top and the bottom
- **Gizmo** at the snap point shows the axis you are using: a ring when you turn, an arrow when you move
- **Key box** in the bottom right shows all keys, the step size and the current offset and rotation

Ward zones and no-build zones are always respected.

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-keys.png)

Every key can be changed in `BepInEx/config/com.morda.anyangle.cfg`. You need no numpad.

| Key | What it does |
|:---|:---|
| **Mouse wheel** | Turn the piece (like vanilla) |
| **Shift + wheel** | Tilt (X axis) |
| **Ctrl + wheel** | Roll (Z axis) |
| **Shift + Ctrl + wheel** | Change the step size |
| **Z + wheel** | Move forward / back |
| **X + wheel** | Move left / right |
| **V + wheel** | Move up / down |
| **Arrows, PgUp, PgDn** | Move with keys (hold to repeat) |
| **Num + / Num -** | Step size up / down |
| **Delete** | Reset offset and rotation |
| **Alt + C** | Copy offset and rotation from the piece you look at |

Steps: 1 cm / 1 degree, 5 cm / 5 degrees, 25 cm / 15 degrees, 1 m / 45 degrees.

While you hold Shift, Ctrl, Z, X or V the wheel belongs to the mod: vanilla turning and camera zoom stay still. Z, X and V also do nothing else in the build menu (no sitting on X).

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-settings.png)

- **Enabled:** turns the mod on or off
- **IgnoreInvalid** (on by default): while a nudge is active, a preview that is marked invalid can still be placed
- **ResetAfterPlace** (off by default): resets offset and rotation after each piece. Off = the same offset stays for the next pieces
- **MaxOffset** (3 m): how far a piece can be moved from its normal spot
- **ShowHud:** shows the key box
- **WheelDepth / WheelSide / WheelHeight** (Z / X / V): the keys you hold for moving with the wheel

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-good-to-know.png)

- Moving goes along the way the piece faces and stays level, so tilting or rolling it never turns "left / right" into up / down. Up / down is always straight up and down
- Pieces you nudge store their offset as an extra value in the world data. The game and other mods ignore it
- This is an early version. Mods that also change the build preview or snapping (for example PlanBuild or snap point mods) may conflict
- A nudged piece that hangs in the air can still collapse if it has no support

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-more-mods-by-morda.png)

| [![Workbenches+](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-workbenches-192.png)](https://thunderstore.io/c/valheim/p/Morda/WorkbenchesPlus/) | [![Forager's Garden](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-foragers-192.png)](https://thunderstore.io/c/valheim/p/Morda/ForagersGarden/) | [![Morda's Gearhold](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-gearhold-192.png)](https://thunderstore.io/c/valheim/p/Morda/MordasGearhold/) | [![Valheim Mannequins](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-mannequins-192.png)](https://thunderstore.io/c/valheim/p/Morda/ValheimMannequins/) |
|:---:|:---:|:---:|:---:|
| **[Workbenches+](https://thunderstore.io/c/valheim/p/Morda/WorkbenchesPlus/)** | **[Forager's Garden](https://thunderstore.io/c/valheim/p/Morda/ForagersGarden/)** | **[Morda's Gearhold](https://thunderstore.io/c/valheim/p/Morda/MordasGearhold/)** | **[Valheim Mannequins](https://thunderstore.io/c/valheim/p/Morda/ValheimMannequins/)** |

| [![ServerRates](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-serverrates-192.png)](https://thunderstore.io/c/valheim/p/Morda/ServerRates/) | [![Smooth Build Repair](https://raw.githubusercontent.com/morda0511/StoreAndCraft/main/thunderstore-assets/store-and-craft/logo-smoothbuildrepair-192.png)](https://thunderstore.io/c/valheim/p/Morda/SmoothBuildRepair/) | [![Store and Craft](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/logo-storeandcraft-192.png)](https://thunderstore.io/c/valheim/p/Morda/StoreAndCraft/) |
|:---:|:---:|:---:|
| **[ServerRates](https://thunderstore.io/c/valheim/p/Morda/ServerRates/)** | **[Smooth Build Repair](https://thunderstore.io/c/valheim/p/Morda/SmoothBuildRepair/)** | **[Store and Craft](https://thunderstore.io/c/valheim/p/Morda/StoreAndCraft/)** |

![](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/dividers/divider-install.png)

1. Install [BepInExPack Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)
2. Put `AnyAngle.dll` in `BepInEx/plugins/AnyAngle/` (or use Thunderstore / r2modman)
3. Servers do not need the mod

![Any Angle](https://raw.githubusercontent.com/morda0511/AnyAngle/main/thunderstore-assets/anyangle/anyangle-footer.png)
