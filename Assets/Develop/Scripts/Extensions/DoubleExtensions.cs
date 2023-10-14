using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DoubleExtensions
{
    public static bool IsHit(this double value) => value > Random.value;
}
