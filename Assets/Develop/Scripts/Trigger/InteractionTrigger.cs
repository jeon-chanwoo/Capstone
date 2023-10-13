using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTrigger : Trigger
{
    private bool isInteracted = false;

    public override void FireTrigger()
    {
        if(!isInteracted)
        {
            isInteracted = true;
            base.FireTrigger();
        }
    }
}
