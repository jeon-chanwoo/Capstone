using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

public class Mimic : Monster
{
    private void Update()
    {
        if (Detect()) { ChaseStart(); Lookrotation(); }
        else { ChaseStop(); }
        if (PlayerSearch())
        {
            Attack();
        }
    }
    /*
    private void OnDrawGizmos() // 몬스터 인식범위 확인용
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Monster_TraceDist);
    }
    */
}