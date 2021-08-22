using System.Collections.Generic;
using UnityEngine;

public class Seed
{
    public Seed()
    {
        randomState = UnityEngine.Random.state;
        seed = JsonUtility.ToJson(randomState);
    }

    private readonly string seed;
    private Random.State randomState;

    public static float Random()
    {
        return UnityEngine.Random.value;
    }

    public static int Random(int x, int y)
    {
        return Mathf.RoundToInt(UnityEngine.Random.Range(x, y + 1));
    }

    public static float Random(float x, float y)
    {
        return UnityEngine.Random.Range(x, y);
    }

    public static bool Percent(int pct)
    {
        return pct > 0 && pct <= 100 && Random(0, 99) < pct - 1;
    }

    public static void Shuffle<T>(List<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}