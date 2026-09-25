# ImmersiveTrader

A Valheim mod with 14 regular traders, hunting contracts, courier shipments and reputation.

## Gameplay

- Two traders in each biome from Meadows through Ashlands. Legendary Mieteg is a separate rare trader.
- Each regular trader offers five shipments and five hunting contracts. Cargo is physical, heavy and cannot pass through portals.
- Carry up to two shipments and two contracts from each issuing trader. Each individual shipment or contract has its own seven Valheim day purchase cooldown.
- Hunting contract scrolls track progress in item metadata, even when dropped and recovered. Completing one grants two levels in its specified skill, up to the skill cap.
- Completing a shipment or contract awards reputation with its issuing trader. Reputation is stored per character, world and trader. The five reputation levels have thresholds of 0, 12, 28, 44 and 64 points.
- Midka additionally sells Minor Healing Mead for 70 coins. The reputation-unlocked vanilla items listed in the design spreadsheet are not yet implemented in the game.
- Trader locations and protection domes are available. Human trader outfits use the Player visual equipment system; NPC environments remain under development.

## Trader outfits

The plugin creates JSON outfit files for all 13 human traders when it starts. Midka's file,
`BepInEx/config/ImmersiveTrader/outfits/midka.json`, uses these defaults:

```json
{
  "Helmet": "HelmetRootCrown",
  "Chest": "ArmorRootChest",
  "Legs": "ArmorRootLegs",
  "RightHand": "StaffGreenRoots",
  "LeftHand": ""
}
```

Midka wears green root equipment and holds a green magic staff. Encek wears the
Deep North Protector set, a Frostfire Sword and a ShieldGold. Tyrron (the existing
Mistlands trader internally identified as `cmok`) wears Flametal armor and carries
ShieldFlametal with MaceGold_FrostFire as the available mace-like weapon. His outfit
is configured in `outfits/cmok.json` to preserve existing save data. The other human traders
wear biome-based sets. Troldad retains the Troll appearance.

Edit prefab names while Valheim is closed and restart the game. An empty slot
removes that item. When upgrading from earlier releases, files containing the
original generated defaults for Midka, Encek and Ragnar are migrated on startup;
customized armor choices are preserved. RightHand and LeftHand are added to older files. Traders have individual greetings,
purchase lines and occasional speech bubbles, as well as descriptions in their hover text.
Trader clothing is rendered on each client: copy customized JSON files to
other players' installations if everyone should see the same look.

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
