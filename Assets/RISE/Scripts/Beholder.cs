using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;

public class Beholder : Monster
{
    private void Update()
    {
        if(Detect())    ChaseStart();
        else ChaseStop();

        if (PlayerSearch()) Die();
    }
}