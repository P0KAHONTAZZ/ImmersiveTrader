using System.Collections;
using System.Collections.Generic;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace ImmersiveTrader;

/// <summary>
/// Central network boundary for multiplayer ImmersiveTrader.
/// RPC handlers are registered on every peer before world login.
/// </summary>
internal static class MultiplayerNetwork
{
    private static CustomRPC? _handshake;
    private static CustomRPC? _reputation;
    private static CustomRPC? _cooldown;
    private static readonly Dictionary<string, int> ReputationCache = new();
    private static readonly Dictionary<string, int> CooldownCache = new();

    internal static void Register()
    {
        if (_handshake != null) return;
        _handshake = NetworkManager.Instance.AddRPC("MultiplayerHandshake", ServerHandshake, ClientHandshake);
        _reputation = NetworkManager.Instance.AddRPC("ReputationState", ServerReputation, ClientReputation);
        _cooldown = NetworkManager.Instance.AddRPC("CooldownState", ServerCooldown, ClientCooldown);
        Plugin.Log.LogInfo("Multiplayer RPC layer registered.");
    }

    internal static bool IsServerAuthority => ZNet.instance != null && ZNet.instance.IsServer();
    internal static bool IsDedicatedServer => ZNet.instance != null && ZNet.instance.IsDedicated();

    private static string ReputationKey(long playerId, string trader)
        => MultiplayerPlayerState.WorldId() + "|" + playerId + "|" + trader;

    internal static bool TryGetCachedReputation(Player player, string trader, out int value)
        => ReputationCache.TryGetValue(ReputationKey(player.GetPlayerID(), trader), out value);

    internal static void CacheReputation(long playerId, string trader, int value)
        => ReputationCache[ReputationKey(playerId, trader)] = value;

    internal static void RequestReputation(Player player, string trader)
    {
        if (_reputation == null || ZRoutedRpc.instance == null || ZNet.instance == null || ZNet.instance.IsServer())
            return;
        var package = new ZPackage();
        package.Write((byte)0); // query
        package.Write(player.GetPlayerID());
        package.Write(trader);
        _reputation.SendPackage(ZRoutedRpc.Everybody, package);
    }

    internal static void PushReputation(long peer, long playerId, string trader, int value)
    {
        if (_reputation == null || !IsServerAuthority) return;
        var package = new ZPackage();
        package.Write((byte)1); // authoritative state
        package.Write(playerId);
        package.Write(trader);
        package.Write(value);
        _reputation.SendPackage(peer, package);
    }

    private static string CooldownKey(long playerId, string trader, string kind, string item)
        => MultiplayerPlayerState.WorldId() + "|" + playerId + "|" + trader + "|" + kind + "|" + item;

    internal static bool TryGetCachedCooldown(Player player, string trader, string kind, string item, out int days)
        => CooldownCache.TryGetValue(CooldownKey(player.GetPlayerID(), trader, kind, item), out days);

    internal static void RequestCooldown(Player player, string trader, string kind, string item, double cooldownDays)
    {
        if (_cooldown == null || ZRoutedRpc.instance == null || ZNet.instance == null || ZNet.instance.IsServer())
            return;
        var package = new ZPackage();
        package.Write((byte)0);
        package.Write(player.GetPlayerID());
        package.Write(trader);
        package.Write(kind);
        package.Write(item);
        package.Write(cooldownDays);
        _cooldown.SendPackage(ZRoutedRpc.Everybody, package);
    }

    internal static void InvalidateCooldown(Player player, string trader, string kind, string item)
        => CooldownCache.Remove(CooldownKey(player.GetPlayerID(), trader, kind, item));

    internal static void SendHandshake()
    {
        if (_handshake == null || ZRoutedRpc.instance == null || ZNet.instance == null || ZNet.instance.IsServer())
            return;
        var package = new ZPackage();
        package.Write(Plugin.ModVersion);
        _handshake.SendPackage(ZRoutedRpc.Everybody, package);
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

    private static IEnumerator ServerReputation(long sender, ZPackage package)
    {
        byte operation = package.ReadByte();
        if (operation != 0 || !IsServerAuthority) yield break;

        long playerId = package.ReadLong();
        string trader = package.ReadString();

        Player? player = FindPlayer(playerId);
        if (player == null)
        {
            Plugin.Log.LogWarning($"Rejected reputation query from peer {sender}: player {playerId} is not present.");
            yield break;
        }

        int value = TraderReputation.GetAuthoritative(player, trader);
        PushReputation(sender, playerId, trader, value);
        yield return null;
    }

    private static IEnumerator ClientReputation(long sender, ZPackage package)
    {
        byte operation = package.ReadByte();
        if (operation != 1) yield break;
        long playerId = package.ReadLong();
        string trader = package.ReadString();
        int value = package.ReadInt();
        CacheReputation(playerId, trader, value);
        yield return null;
    }

    private static IEnumerator ServerCooldown(long sender, ZPackage package)
    {
        byte operation = package.ReadByte();
        if (operation != 0 || !IsServerAuthority) yield break;
        long playerId = package.ReadLong();
        string trader = package.ReadString();
        string kind = package.ReadString();
        string item = package.ReadString();
        double cooldownDays = package.ReadDouble();

        Player? player = FindPlayer(playerId);
        if (player == null) yield break;

        bool canBuy = ItemPurchaseCooldown.CanBuyAuthoritative(player, trader, kind, item, cooldownDays, out int days);
        var response = new ZPackage();
        response.Write((byte)1);
        response.Write(playerId);
        response.Write(trader);
        response.Write(kind);
        response.Write(item);
        response.Write(canBuy ? 0 : days);
        _cooldown?.SendPackage(sender, response);
        yield return null;
    }

    private static IEnumerator ClientCooldown(long sender, ZPackage package)
    {
        if (package.ReadByte() != 1) yield break;
        long playerId = package.ReadLong();
        string trader = package.ReadString();
        string kind = package.ReadString();
        string item = package.ReadString();
        int days = package.ReadInt();
        CooldownCache[CooldownKey(playerId, trader, kind, item)] = days;
        yield return null;
    }

    internal static Player? FindPlayer(long playerId)
    {
        foreach (Player player in Player.GetAllPlayers())
            if (player != null && player.GetPlayerID() == playerId)
                return player;
        return null;
    }
}
