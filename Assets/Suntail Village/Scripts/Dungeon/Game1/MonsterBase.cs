using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterBase : MonoBehaviour
{
    public Action<MonsterBase> OnBossDead;
    public Action<float, float> OnDamaged;

    public abstract bool isDead { get; }
    public abstract float maxHp { get; }
    public abstract float currentHp { get; }

    //public bool isWorking;
    //public bool isDead { get; protected set; }

    public abstract void TakeDamage(float damageAmount);
}
