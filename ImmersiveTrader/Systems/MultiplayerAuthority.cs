namespace ImmersiveTrader;

/// <summary>
/// Guards mutations that must be authoritative in multiplayer. Single-player/host keeps
/// the existing direct path. Remote clients will use request/response RPC transaction paths.
/// </summary>
internal static class MultiplayerAuthority
{
    internal static bool CanMutatePersistentState()
        => ZNet.instance == null || ZNet.instance.IsServer();

    internal static void RequireServer(string operation)
    {
        if (!CanMutatePersistentState())
            throw new System.InvalidOperationException(
                $"ImmersiveTrader multiplayer authority violation: client attempted {operation}.");
    }
}
