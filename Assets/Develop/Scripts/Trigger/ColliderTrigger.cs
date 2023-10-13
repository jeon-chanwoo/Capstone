using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderTrigger : Trigger
{
    private Collider _collider;

    [SerializeField] private bool _preventDoubleFire;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(_collider != null && _preventDoubleFire)
            {
                _collider.enabled = false;
            }

            FireTrigger();
        }
    }
}
