# ImmersiveTrader

A Valheim mod with 14 regular traders, hunting contracts, courier shipments and reputation.

## Gameplay

- Two traders in each biome from Meadows through Ashlands. Legendary Mieteg is a separate rare trader.
- Each regular trader offers five shipments and five hunting contracts. Cargo is physical, heavy and cannot pass through portals.
- Carry up to two shipments and two contracts from each issuing trader. Each individual shipment or contract has its own seven Valheim day purchase cooldown.
- Hunting contract scrolls track progress in item metadata, even when dropped and recovered. Completing one grants two levels in its specified skill, up to the skill cap.
- Completing a shipment or contract awards reputation with its issuing trader. Reputation is stored per character, world and trader. The five reputation levels have thresholds of 0, 12, 28, 44 and 64 points.
- Midka additionally sells Minor Healing Mead for 70 coins. The reputation-unlocked vanilla items listed in the design spreadsheet are not yet implemented in the game.
- Trader locations, protection domes and the player-like trader visual prototype are under active development. The experimental manual helmet mounting was disabled because it distorted NPC heads.

## Trader outfits

The plugin creates JSON outfit files for all 13 human traders at startup (no world visit required), for example
`BepInEx/config/ImmersiveTrader/outfits/midka.json`:

```json
{
  "Helmet": "HelmetLeather",
  "Chest": "ArmorLeatherChest",
  "Legs": "ArmorLeatherLegs"
}
```

Edit prefab names while the game is closed, then restart Valheim. An empty `Helmet`
hides the helmet. The game renders helmets through the Player visual's native
`VisEquipment` when available; it never scales an item-drop model onto the
head. A missing native equipment component leaves the NPC without a helmet and
writes a warning to `BepInEx/LogOutput.log`. The trader interaction shell and
world location remain unchanged. On multiplayer clients, copy the same outfit
files to each player's machine for consistent appearance.

## Requirements

- Valheim, BepInEx and Jotunn. Compile against the locally installed Valheim assemblies.

## Install on Windows

Close Valheim, then run in PowerShell from the repository directory:

```powershell
cd C:\Users\PC\Desktop\AlbionGit\ImmersiveTrader
git fetch origin feature/contract-world-model
git restore --source=FETCH_HEAD -- .
powershell -ExecutionPolicy Bypass -File .\Build-And-Install-Verified.ps1
```

The verified installer checks the source and bundled images, builds the DLL, moves duplicate copies aside and compares the installed DLL SHA-256 with the build output. A successful run prints `BUILD OK - DLL COPIED`.

For a multiplayer session, all participants must use a compatible version. Server-wide authority and persistence are still being developed.
