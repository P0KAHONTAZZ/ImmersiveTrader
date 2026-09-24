# Ikony kontraktów i przyszły system reputacji

## Zwoje kontraktów

W projekcie jest 70 kontraktów i 12 różnych nagradzanych umiejętności: Axes, Blocking, Bows, Clubs, Jump, Knives, Pickaxes, Polearms, Run, Sneak, Spears, Swords.

Projekt graficzny: jeden czytelny zwój pergaminu z pustym medalionem pośrodku. Na medalionie ma się znaleźć ikona właściwej umiejętności pobrana z aktualnie uruchomionej gry Valheim, zamiast ręcznie narysowanego przybliżenia. Przed integracją sprawdzić przez `Skills.GetSkillDef(...).m_icon` obecny interfejs gry oraz obsługę tekstur sprite atlasu. Różne kontrakty nagradzające tę samą umiejętność mogą dzielić ikonę; identyfikator i postęp pozostają w metadanych przedmiotu.

Wzór wizualny wskazany przez użytkownika: https://kg.sayless.eu/ves/#Item%20Enchantment (sekcje Enchantment Scrolls i Item Enchantment). Tworzyć własną grafikę; nie kopiować zasobów tego moda do projektu.

## Zgodność na przyszłość

Użytkownik dostarczył `kg.ValheimEnchantmentSystem.dll` jako materiał do późniejszego systemu reputacji. SHA-256 dostarczonej DLL: `399ee93f84a857c2ffb5f40b48251f71e1f88367ff3c14377bec692db03d717d`. Nie dołączać cudzej DLL do repozytorium i nie wprowadzać zależności bez analizy API oraz wersji moda. Reputacja jest odrębnym etapem pracy.
