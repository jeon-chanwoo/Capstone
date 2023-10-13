using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringExtensions
{
    public static Color ToColor(this string s)
    {
        Color color = Color.white;
        ColorUtility.TryParseHtmlString(s, out color);
        return color;
    }
}
