# ImmersiveTrader — zasady pracy

Mod do Valheim (BepInEx 5.4.23.5, Jotunn 2.30.0, Valheim 1.0.16 / network 40).

## Baza i branch
- Bezpieczna baza: `3c8560e9e96d28d6d54d5ab3bd684ad3c3c55d78`.
- Branch roboczy: `feature/enhancement-scroll-items-v2` (wychodzi z 3c8560e).
- NIE korzystać z późniejszych commitów `feature/status-effects` (e735b91..e20a964) ani z `EnhancementScrollRegistry`.

## Zamrożone (Stable Core) — nie zmieniać
Traderzy/NPC (14), sklepy i towary, progresja bossów, reputacja, kontrakty (70 fizycznych),
24 skille i nagrody +2, skalowanie kontraktów, cargo (910 tras), persistence, wyposażenie NPC, lokacje.
Mechanizmu kontraktów nie używać do scrolli.

## Status HUD — zatwierdzony
- Atlas: `Assets/enhancement-status-icons-approved.png` (1920×128, 15 ikon po 128 px). Nie zmieniać.
- HUD: nazwa + ikona + timer, bez opisu i tooltipa.
- 15 buffów = 1800 s. Rested = natywny status i ikona Valheima, 1200 s.
- Vitality = aktualne Max HP × 1.10, Focus = aktualne Max Eitr × 1.15 (dynamicznie, reaguje na jedzenie).
  Wygaśnięcie tylko przycina HP/Eitr do nowego maksimum — nigdy nie zadaje obrażeń.
  UWAGA: na 3c8560e dynamiczne Vitality/Focus NIE są jeszcze zaimplementowane.

## Etap bieżący: 16 fizycznych Enhancement Scrolls
Scroll w inventory → użycie → zużycie 1 szt. → status → ikona HUD → 30 min (Rested 20 min).
- Ikona inventory ≠ ikona HUD. Inventory: `Assets/enhancement-scroll-inventory-icons.png` (2048×128, 16 × 128 px).
  Kolejność: Embers, Frost, Storm, Venom, Spirit, Lumberjack, Miner, Burden, Vitality, Endurance,
  Focus, Craftsman, Wanderer, Pathfinder, Hunter, Rested.
- Na ziemi: tymczasowo model 3D kontraktu (klon, nie modyfikacja oryginału).
- Użycie przez natywny flow Consumable (`m_consumeStatusEffect`), bez patchowania `UseItem`.

## Lekcja z poprzedniej awarii
Log: tysiące `IndexOutOfRangeException` w `ItemDrop.ItemData.GetIcon` ← `InventoryGrid.UpdateGui`,
poprzedzone `Enhancement scroll inventory icon atlas missing`. Przyczyna: item zarejestrowany z pustą
tablicą `m_shared.m_icons`. Zasady:
- Każdy item MUSI mieć `m_icons` z co najmniej 1 spritem; przy braku atlasu użyć ikony fallback
  (np. ikony kontraktu), nigdy pustej tablicy.
- Assety używane przez kod muszą być w gicie, a `EmbeddedResource` nie może mieć cichego `Condition="Exists(...)"`
  dla wymaganych plików.

## Build
- `Environment.props` lokalnie (VALHEIM_INSTALL). Build: `Build-And-Install-Verified.ps1`.
- DLL: `ImmersiveTrader\bin\Debug\netstandard2.1\ImmersiveTrader.dll` → `Valheim\BepInEx\plugins\`.
- Log gry: `Valheim\BepInEx\LogOutput.log`.
