using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IListExtensions
{
    public static T Front<T>(this IList<T> list) => list[0];
    public static T Last<T>(this IList<T> list) => list[list.Count - 1];
    public static IList<T> RandomPick<T>(this IList<T> list, int maxCount)
    {
        List<T> picks = new List<T>(list);

        int pickCount = Mathf.Min(list.Count, maxCount);
        for(int i=0; i < list.Count - pickCount; i++)
        {
            picks.RemoveAt(Random.Range(0, picks.Count));
        }

        return picks;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static bool ContainsAll(this IList<string> list, string[] items)
    {
        foreach(var item in items)
        {
            if(!list.Contains(item.Trim()))
                return false;
        }
        return true;
    }

    public static bool ContainsAll<T>(this IList<T> list, T[] items)
    {
        foreach(var item in items)
        {
            if(!list.Contains(item))
                return false;
        }
        return true;
    }
}
