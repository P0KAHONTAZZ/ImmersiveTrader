# Proponowane lokacje More World Locations AIO dla 14 traderów

Status: kandydaci zweryfikowani w aktualnym `LocationDefinitions.cs` repozytorium autora. Link do wydania 2.0.7 dotyczy starszej listy 119 lokacji; przed włączeniem mapowania trzeba potwierdzić prefabrykaty w ZIP wydania 2.0.7. Żaden cudzy asset nie jest dołączony do ImmersiveTrader.

| Trader | Biom | Prefab MWL | Motyw |
|---|---|---|---|
| Midka | Meadows | MWL_MeadowsHouse2 | Dom medyczki |
| Troldad | Meadows | MWL_MeadowsSawmill1 | Tartak i drewno |
| Grimvald | Black Forest | MWL_ForestForge1 | Warsztat |
| Rudy Warg | Black Forest | MWL_ForestCamp1 | Obóz myśliwego |
| Mokra Dzika | Swamp | MWL_SwampHouse1 | Bagienny dom |
| Encek | Swamp | MWL_SwampTreeHut1 | Chata na palach |
| Hrothgar | Mountains | MWL_StoneHall1 | Kamienna hala |
| Ylva Frost | Mountains | MWL_StoneTavern1 | Górska karczma |
| Bjarki Goldtooth | Plains | MWL_PlainsTavern1 | Karczma |
| Ragnar Turnipson | Plains | MWL_PlainsCamp1 | Rolniczy obóz |
| Ćmok | Mistlands | MWL_DvergrHouse1 | Dom Dvergr |
| Grelka | Mistlands | MWL_DvergrHouseWood1 | Drewniany dom Dvergr |
| Spalony Zenek | Ashlands | MWL_AshlandsFort1 | Fort |
| Skjold Cinderborn | Ashlands | MWL_AshlandsFort2 | Fort rzemieślnika |

Projekt integracji: opcjonalna zależność od zainstalowanego MWL, bez kopiowania bundle/prefabów. Powiększyć radius i dostosować warunki terenu do oryginalnej lokacji, osadzić jednego NPC w sprawdzonym punkcie interakcji, zapobiec duplikatom innych NPC i wydarzeń, zachować `it findall` 14/14 oraz fallback do własnych lokacji, jeśli MWL nie ma. Wymagane testy nowych światów oraz multiplayer. Sprawdzić licencję i otrzymać zgodę autora przed redystrybucją lub przeróbką assetów.
