using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Attack : MonoBehaviour
{
    public BoxCollider AttackArea;
    public int damage;
    public float rate = 0.5f;

    public void use()
    {
        StopCoroutine("THS_Attack1");
        StartCoroutine("THS_Attack1");
    }

    IEnumerator THS_Attack1()
    {
        AttackArea.enabled = true;
        yield return new WaitForSeconds(rate);
        AttackArea.enabled = false;
    }
}
