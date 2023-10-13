using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloatExtensions
{
    public static bool IsHit(this float value) => value > Random.value;
}
