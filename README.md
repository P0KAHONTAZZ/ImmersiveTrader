# ImmersiveTrader

Valheim mod: sieć immersyjnych traderów i questów transportowych.

## Aktualny zakres
- 14 stałych traderów: po 2 w Meadows, Black Forest, Swamp, Mountains, Plains, Mistlands i Ashlands.
- Legendary Mieteg jako sekretny, rzadki trader.
- 70 ciężkich towarów transportowych, po 5 u każdego zwykłego handlarza; 910 tras dostawy.
- Towarów nie można teleportować. Można mieć maksymalnie 2 aktywne przesyłki od jednego nadawcy.
- Pięć rodzajów ikon opakowań: skrzynia, beczka, worek, pakunek i kosz. Na ziemi towary mają model skrzyni.
- 70 fizycznych kontraktów łowieckich obejmuje 24 umiejętności Valheim 1.0. Postęp i umiejętność nagrody są zapisane na zwoju i pozostają po wyrzuceniu oraz podniesieniu.
- Blood Magic występuje tylko u Grelki w Mistlands. Zwoje wystawione przed rozszerzeniem umiejętności zachowują dotychczasową nagrodę.
- Nagrody skalują się wg odległości tierów biomów: x1 / x2 / x4 / x6, z twardym capem x6.
- Bone Fragments są wykluczone z nagród.
- Midka jest doświadczonym wojennym healerem i jako jedyna ma zwykły towar w sklepie: Minor Healing Mead za 70 Coins.
- Pozostali handlarze oferują tylko towary transportowe i kontrakty łowieckie.
- Troldad jest drwalem, myśliwym i alkoholikiem; zapowiada inną nagrodę niż faktycznie daje.

## Stack
- BepInEx
- Jotunn

## Instalacja na Windows

Zamknij Valheim. W PowerShell przejdź do katalogu repozytorium i pobierz aktualną gałąź:

```powershell
cd C:\Users\PC\Desktop\AlbionGit\ImmersiveTrader
git fetch origin feature/contract-world-model
git restore --source=FETCH_HEAD -- Assets/contract-scroll-simple.png Assets/cargo-packages.png ImmersiveTrader/ImmersiveTrader.csproj ImmersiveTrader/Systems/ContractIconRegistry.cs ImmersiveTrader/Systems/ContractWorldModel.cs ImmersiveTrader/Systems/ContractRegistry.cs ImmersiveTrader/Systems/RewardRegistry.cs ImmersiveTrader/Systems/CargoPresentation.cs ImmersiveTrader/Systems/TreasureRegistry.cs Build-And-Install-Verified.ps1
powershell -ExecutionPolicy Bypass -File .\Build-And-Install-Verified.ps1
```

Skrypt weryfikuje skompilowaną DLL przez SHA256. Dodatkowe kopie `ImmersiveTrader.dll` przenosi do `BepInEx\disabled-ImmersiveTrader`.
