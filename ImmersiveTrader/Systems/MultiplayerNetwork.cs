using System.Collections;
using Jotunn.Entities;
using Jotunn.Managers;

namespace ImmersiveTrader;

/// <summary>
/// Central network boundary for multiplayer ImmersiveTrader.
/// Gameplay RPCs are registered here so every peer has the same routes before world login.
/// </summary>
internal static class MultiplayerNetwork
{
    private static CustomRPC? _handshake;

    internal static void Register()
    {
        if (_handshake != null) return;
        _handshake = NetworkManager.Instance.AddRPC(
            "MultiplayerHandshake",
            ServerHandshake,
            ClientHandshake);
        Plugin.Log.LogInfo("Multiplayer RPC layer registered.");
    }

    internal static bool IsServerAuthority
        => ZNet.instance != null && ZNet.instance.IsServer();

    internal static bool IsDedicatedServer
        => ZNet.instance != null && ZNet.instance.IsDedicated();

    internal static void SendHandshake()
    {
        if (_handshake == null || ZRoutedRpc.instance == null || ZNet.instance == null || ZNet.instance.IsServer())
            return;

        var package = new ZPackage();
        package.Write(Plugin.ModVersion);
        _handshake.SendPackage(ZRoutedRpc.instance.GetServerPeerID(), package);
    }

    private static IEnumerator ServerHandshake(long sender, ZPackage package)
    {
        string clientVersion = package.ReadString();
        Plugin.Log.LogInfo($"Multiplayer handshake from peer {sender}: client={clientVersion}, server={Plugin.ModVersion}");

        var response = new ZPackage();
        response.Write(Plugin.ModVersion);
        response.Write(true);
        _handshake?.SendPackage(sender, response);
        yield return null;
    }

    private static IEnumerator ClientHandshake(long sender, ZPackage package)
    {
        string serverVersion = package.ReadString();
        bool accepted = package.ReadBool();
        Plugin.Log.LogInfo($"Multiplayer handshake response from peer {sender}: server={serverVersion}, accepted={accepted}");
        yield return null;
    }
}
