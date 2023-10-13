using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public event Action OnFiredTrigger;

    public virtual void FireTrigger()
    {
        OnFiredTrigger?.Invoke();
    }
}
