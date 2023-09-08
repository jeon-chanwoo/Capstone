using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Weapon : MonoBehaviour
{
    public enum Type { THS };
    public Type type;
    public int damage;
    public float rate;
    public BoxCollider AttackArea;

    public void use()
    {
        if (type == Type.THS)
        {
            StopCoroutine("THS_Attack1");
            StartCoroutine("THS_Attack1");
        }
    }

    IEnumerator THS_Attack1()
    {
        AttackArea.enabled = true;
        yield return new WaitForSeconds(rate);
        AttackArea.enabled = false;
    }
}
