using System;
using System.Text;

namespace ImmersiveTrader;

/// <summary>
/// Stable identity for multiplayer state. Reputation and transaction state remain
/// per character; the server merely owns the authoritative copy for the current world.
/// </summary>
internal static class MultiplayerPlayerState
{
    internal static long PlayerId(Player player) => player.GetPlayerID();

    internal static string WorldId()
    {
        var net = ZNet.instance ?? throw new InvalidOperationException("World is not loaded.");
        foreach (string method in new[] { "GetWorldUID", "GetWorldName" })
        {
            var info = net.GetType().GetMethod(method,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);
            if (info == null || info.GetParameters().Length != 0) continue;
            string? value = info.Invoke(net, null)?.ToString();
            if (!string.IsNullOrEmpty(value)) return value;
        }
        throw new InvalidOperationException("Cannot identify current world.");
    }

    internal static string Key(Player player, params string[] parts)
        => Key(PlayerId(player), parts);

    internal static string Key(long playerId, params string[] parts)
    {
        string plain = WorldId() + "|" + playerId + "|" + string.Join("|", parts);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plain));
    }
}
