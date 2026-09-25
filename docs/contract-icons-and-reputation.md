# Contract icons and reputation

## Contract scrolls

The mod defines 70 hunting contracts and covers 24 Valheim skills. Each scroll combines a parchment graphic with the current game's icon for its reward skill. The contract ID, issuer, required kills, progress and reward skill are kept as item metadata, so dropping and recovering a scroll preserves its state.

The reference provided for the visual direction was https://kg.sayless.eu/ves/#Item%20Enchantment. ImmersiveTrader uses its own art and does not copy assets from that mod.

## Reputation

Completing a hunting contract grants reputation to its issuing trader. Completing a delivery also grants reputation to the shipment's original trader. Levels 1 through 5 unlock at 0, 12, 28, 44 and 64 points. Reputation is recorded per character, world and trader. Shop stock unlocked by reputation and one-time level 5 gifts are planned in the spreadsheet but are not yet implemented in game.

The user supplied `kg.ValheimEnchantmentSystem.dll` as background for possible future integration. Supplied file SHA-256: `399ee93f84a857c2ffb5f40b48251f71e1f88367ff3c14377bec692db03d717d`. It must not be distributed with this project without permission or API compatibility review.

## Hunting balance

Contracts have a single fixed target count for each monster type. Examples: 30 Greydwarf Shamans or Brutes, 15 Trolls or Wraiths, and 10 Abominations. Each completed contract grants exactly two skill levels relative to the player's current level, capped at 100. Existing scrolls retain their stamped target and reward skill. Future reputation bonuses to reduce required kills are not active.
