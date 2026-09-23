using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveTrader;

public static class TroldadDialogue
{
    private static readonly string[] Promises =
    {
        "deer hides", "fine wood", "honey", "meat", "resin", "feathers"
    };

    private static readonly string[] Excuses =
    {
        "...well. I ran out of that.",
        "Don't look at me like that. It's practically the same thing.",
        "I may have had a drink when I packed it.",
        "...wait. Was that mine?",
        "No refunds. Especially not today."
    };

    public static string GetLie(string actualItem, int actualAmount)
    {
        string promise;
        do promise = Promises[Random.Range(0, Promises.Length)];
        while (actualItem.ToLowerInvariant().Contains(promise.Replace(" ", "").ToLowerInvariant()));

        string excuse = Excuses[Random.Range(0, Excuses.Length)];
        return $"Troldad: I promised you {promise}. {excuse}\nReceived: {actualAmount} {actualItem}";
    }
}
