using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    public static Color SetAlpha(this Color color, float a)
    {
        color.a = a;
        return color;
    }
}
