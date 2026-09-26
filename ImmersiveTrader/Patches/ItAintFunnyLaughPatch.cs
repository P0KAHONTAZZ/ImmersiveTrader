using System;
using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Networking;

namespace ImmersiveTrader;

/// <summary>
/// Attaches the nokidding audio controller to the local player. No emote/animation is used.
/// </summary>
[HarmonyPatch(typeof(Player), "FixedUpdate")]
internal static class ItAintFunnyLaughPatch
{
    private static void Postfix(Player __instance)
    {
        if (__instance != Player.m_localPlayer) return;
        if (__instance.GetComponent<ItAintFunnyAudio>() == null)
            __instance.gameObject.AddComponent<ItAintFunnyAudio>();
    }
}

internal sealed class ItAintFunnyAudio : MonoBehaviour
{
    private const string MaleResource = "ImmersiveTrader.Assets.ItAintFunny_Male.ogg";
    private const string FemaleResource = "ImmersiveTrader.Assets.ItAintFunny_Female.ogg";

    private AudioSource? _source;
    private AudioClip? _male;
    private AudioClip? _female;
    private Player? _player;
    private float _nextLaugh;
    private bool _wasActive;
    private static readonly FieldInfo? ModelIndexField =
        AccessTools.Field(typeof(Player), "m_modelIndex");

    private void Awake()
    {
        _player = GetComponent<Player>();
        _source = gameObject.AddComponent<AudioSource>();
        _source.playOnAwake = false;
        _source.loop = false;
        _source.spatialBlend = 1f;
        _source.rolloffMode = AudioRolloffMode.Linear;
        _source.minDistance = 2f;
        _source.maxDistance = 25f;
        _source.dopplerLevel = 0f;
        StartCoroutine(LoadClips());
    }

    private IEnumerator LoadClips()
    {
        yield return LoadEmbeddedOgg(MaleResource, "ImmersiveTrader_ItAintFunny_Male.ogg", c => _male = c);
        yield return LoadEmbeddedOgg(FemaleResource, "ImmersiveTrader_ItAintFunny_Female.ogg", c => _female = c);
    }

    private static IEnumerator LoadEmbeddedOgg(string resource, string fileName, Action<AudioClip> assign)
    {
        var asm = typeof(ItAintFunnyAudio).Assembly;
        using var stream = asm.GetManifestResourceStream(resource);
        if (stream == null)
        {
            ImmersiveTraderPlugin.Log.LogError($"Missing embedded audio resource: {resource}");
            yield break;
        }

        string dir = Path.Combine(Paths.CachePath, "ImmersiveTrader");
        Directory.CreateDirectory(dir);
        string path = Path.Combine(dir, fileName);
        using (var file = File.Create(path))
            stream.CopyTo(file);

        using var req = UnityWebRequestMultimedia.GetAudioClip("file:///" + path.Replace('\\', '/'), AudioType.OGGVORBIS);
        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            ImmersiveTraderPlugin.Log.LogError($"Failed loading {resource}: {req.error}");
            yield break;
        }

        var clip = DownloadHandlerAudioClip.GetContent(req);
        clip.name = Path.GetFileNameWithoutExtension(fileName);
        assign(clip);
    }

    private void Update()
    {
        if (_player == null || _source == null || _player != Player.m_localPlayer) return;

        bool active = EnhancementStatusRegistry.Has(_player, "nokidding");
        if (!active)
        {
            if (_wasActive) _source.Stop();
            _wasActive = false;
            _nextLaugh = 0f;
            return;
        }

        if (!_wasActive)
        {
            _wasActive = true;
            _nextLaugh = Time.time + UnityEngine.Random.Range(2f, 5f);
            return;
        }

        if (_source.isPlaying || Time.time < _nextLaugh) return;

        AudioClip? clip = IsFemale() ? _female : _male;
        if (clip == null) return;

        _source.clip = clip;
        _source.Play();
        _nextLaugh = Time.time + clip.length + UnityEngine.Random.Range(2f, 5f);
    }

    private bool IsFemale()
    {
        if (_player == null || ModelIndexField == null) return false;
        try
        {
            object? value = ModelIndexField.GetValue(_player);
            return value is int index && index == 1;
        }
        catch
        {
            return false;
        }
    }
}
