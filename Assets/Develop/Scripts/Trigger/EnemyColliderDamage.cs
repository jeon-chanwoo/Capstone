using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyColliderDamage : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _duration;

    private List<Collider> _damagedColliders = new List<Collider>();

    private float _time;

    private void OnEnable()
    {
        _time = Time.time;
        _damagedColliders.Clear();
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_time + _duration < Time.time)
            return;

        if(other.CompareTag("Player") && !_damagedColliders.Contains(other))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(_damage);

            _damagedColliders.Add(other);
        }
    }
}
