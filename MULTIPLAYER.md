# ImmersiveTrader multiplayer migration

Stable single-player baseline: `stable/immersivetrader-0.5-nokidding` at `948ff858b079aa5bf861df7df658cbdca98f2f09`.

Development branch: `feature/multiplayer`.

## Rules

- ImmersiveTrader and Jotunn are required on the server and every client.
- Network compatibility is enforced with `EveryoneMustHaveMod` and `VersionStrictness.Minor`.
- Existing gameplay values, trader inventory, reputation thresholds, contracts, cargo, scroll effects and visuals must not be rebalanced as part of the multiplayer migration.
- Character-specific progression remains character-specific.
- Shared world entities/locations must be authoritative and synchronized through Valheim networking.
- Persistent gameplay state must not depend on one client's local BepInEx config directory.

## Audit findings / migration order

1. **Compatibility gate** — DONE. Prevent mixed mod/no-mod sessions and incompatible minor versions.
   - Central Jötunn RPC layer registered during plugin startup.
   - Gameplay-affecting BepInEx config entries are server-synchronized/admin-only.
2. **Persistence authority** — REQUIRED. `TraderReputation`, `ItemPurchaseCooldown`, and `TraderTrustReward` currently persist to local files under `BepInEx/config`. In multiplayer this can diverge per machine. Move authoritative writes/validation to the server and key records by world + player + trader/item.
3. **Quest state** — REQUIRED. `QuestState.ActiveByPlayer` is process-local memory. Cargo/contract issuance, completion and rewards need server validation and reconnect-safe state where applicable.
4. **Transactions** — REQUIRED. Trader purchases, cargo delivery, contract completion, reputation awards and one-time Trusted rewards must use request -> server validation -> authoritative result. Client UI must not be the authority.
5. **World NPC/location ownership** — IN PROGRESS. TraderLocationAnchor now instantiates/destroys trader NPCs and Jackie only on the server/host; clients must receive the network objects instead of creating local duplicates. Runtime host+client verification still required.
6. **Kill credit** — REQUIRED. Contract kill progress must attribute the kill to the correct player and avoid each client independently granting progress/rewards.
7. **Custom item/status assets** — VERIFY. All peers receive identical prefab registrations. `nokidding` audio remains a local presentation effect of the status; gameplay state is the status itself.
8. **Configuration** — REQUIRED. Gameplay-affecting server settings must be server-authoritative/synchronized; purely visual/client settings may remain local.
9. **Dedicated server** — REQUIRED. No code path may assume `Player.m_localPlayer`, HUD, audio, graphics, or inventory UI exists on the dedicated server.
10. **Test matrix** — host+client, dedicated server+2 clients, reconnect, death/respawn, simultaneous purchase, simultaneous cargo hand-in, simultaneous contract kill, separate reputation per character, boss progression per character, world restart.

## Invariants to preserve

- 14 traders and current stock/progression.
- Reputation levels and points.
- Personal boss progression semantics.
- Cargo and contract mechanics/rewards.
- Purchase cooldown semantics.
- Trusted reward once per character/world/trader.
- Enhancement scroll mechanics and approved art.
- `nokidding`: 20 minutes, gendered laugh audio, no animation/action lock.
- Troldad remains stationary at his accepted spawn behavior.
