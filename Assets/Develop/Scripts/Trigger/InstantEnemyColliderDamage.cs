using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantEnemyColliderDamage : MonoBehaviour
{
    [SerializeField] private float _damage;
    [SerializeField] private float _duration;

    private List<Collider> _damagedColliders = new List<Collider>();

    private void OnEnable()
    {
        _damagedColliders.Clear();

        StartCoroutine(InactiveCoroutine());
    }

    private IEnumerator InactiveCoroutine()
    {
        yield return new WaitForSeconds(_duration);

        gameObject.SetActive(false);
    }

    public void SetDamage(float damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_damagedColliders.Contains(other))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(_damage);

            _damagedColliders.Add(other);
        }
    }
}
