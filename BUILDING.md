# Building ImmersiveTrader

The project must be compiled against the DLLs from the currently installed Valheim build.

## Required locally
1. Valheim
2. BepInExPack for Valheim
3. Jötunn installed in BepInEx/plugins
4. .NET SDK / Visual Studio with C# tooling

Do **not** commit Valheim DLLs to this repository.

## Setup
Copy `Environment.props.example` to `Environment.props` and set `VALHEIM_INSTALL`.

The required references come from:
- `BepInEx/core`
- `BepInEx/plugins`
- `valheim_Data/Managed`

## First validation target
The first local build is expected to reveal any API differences between this source and the exact Valheim/Jötunn build installed on the test machine. Send the complete compiler output back into the development loop; fix compile errors before testing world generation.

## Runtime test order
1. New disposable world.
2. Confirm plugin loads without red BepInEx errors.
3. Confirm five treasure prefabs exist and weigh 80.
4. Confirm treasure is blocked by portals.
5. Confirm 14 regular trader locations generate.
6. Confirm Midka alternate interaction buys Minor Healing Mead.
7. Confirm trader issues a shipment.
8. Restart game and confirm shipment metadata survives.
9. Die with shipment and recover tombstone; confirm metadata survives.
10. Deliver to another trader and verify x1/x2/x4/x6 reward.
11. Verify same-source trader refuses delivery.
12. Verify progression lock.
13. Multiplayer/server validation.
14. Legendary Mieteg activation and reveal logic (after runtime scheduler is completed).

## Known pre-runtime work
- Cooldown currently has an in-memory implementation and still needs persistent server-side storage.
- Legendary Mieteg prepared locations and reward routing exist; timed activation/reveal is not complete.
- Custom art/asset bundles are not yet included.
