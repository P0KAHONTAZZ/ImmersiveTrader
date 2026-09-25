using UnityEngine;

namespace ImmersiveTrader.Components;

/// <summary>
/// Metadata for the parallel NPC-look test path. The prefab itself is a native Hildir
/// NPC shell; only presentation from VisualSource will be transplanted onto it later.
/// This deliberately keeps monster Character/AI/network behaviour out of the shell.
/// </summary>
public sealed class NpcLookPrototype : MonoBehaviour
{
    public string VisualSource = string.Empty;
    public float VisualScale = 1f;
}
