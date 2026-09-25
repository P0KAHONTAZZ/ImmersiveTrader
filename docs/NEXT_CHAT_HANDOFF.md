# ImmersiveTrader development handoff (2026-09-25)

Repository: `P0KAHONTAZZ/ImmersiveTrader`, branch `feature/contract-world-model`. Earlier stable reference: `stable-patch-2026-09-24`. The user builds on Windows at `C:\Users\PC\Desktop\AlbionGit\ImmersiveTrader` with `Build-And-Install-Verified.ps1`. This environment has no local Valheim assemblies or .NET SDK; do not claim a successful local game build.

## Working features reported by the user

- 14/14 generated traders; shipments, contracts and reward delivery work.
- Seven Valheim day per-item purchase cooldown; two active contracts and shipments per issuer.
- Reputation survives relog; console add and reset commands work.
- The Haldor-style protection dome is visible and enemies do not attack.
- `it goto midka` works.

## Current visuals

The experimental NPCLOOK prototype mounts Player's `Visual` hierarchy on a Hildir-based shell. Body and leg equipment are skinned to the copied visual. A hand-positioned helmet grew far too large and later clipped inside Midka's head. The manual helmet mounting was disabled in `PlayerLikeNpcVisual.cs` in commit `78c017d`; the user has not yet installed or tested that change. Revisit headwear only after finding an owner-driven, character-equipment approach. Troldad uses a scaled native Troll body.

## English language pass

The current branch translates 70 contract titles and descriptions, 70 cargo labels, tooltips, reputation labels, in-game notifications, the build script and documentation into English. Trader IDs, prefab names, custom-data keys, serialized data and monster targets must not change when adjusting language. Cargo package icon selection must follow the English cargo labels.

## Buildings and third-party assets

The user's earlier house experiments looked broken. They provided More World Locations AIO as inspiration; candidate locations are documented in `docs/mwl-location-candidates.md`. Its All Rights Reserved license does not permit bundling those assets without the author's written permission. Use original, independently designed locations and native Valheim pieces if no permission is obtained.

## Design spreadsheet

https://docs.google.com/spreadsheets/d/1dOLE7p1TyOh0TZw6MiFiDV8B69KYI0ORzEuD73kQYeM/edit. `Reputation Stock` contains a design for vanilla item offers at levels 2–5 and a one-time level 5 gift per trader. These offers and gifts are not yet implemented in the DLL.

## User build command

```powershell
cd C:\Users\PC\Desktop\AlbionGit\ImmersiveTrader
git fetch origin feature/contract-world-model
git restore --source=FETCH_HEAD -- .
powershell -ExecutionPolicy Bypass -File .\Build-And-Install-Verified.ps1
```
