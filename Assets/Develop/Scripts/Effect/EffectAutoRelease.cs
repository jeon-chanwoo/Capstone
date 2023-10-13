using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAutoRelease : MonoBehaviour
{
    [SerializeField] private float _delay = 2f;

    private Effect _effect;

    private void Awake()
    {
        _effect = GetComponent<Effect>();
    }

    private void OnEnable()
    {
        StartCoroutine(ReleaseCoroutine());
    }

    private IEnumerator ReleaseCoroutine()
    {
        yield return new WaitForSeconds(_delay);

        EffectPoolManager.instance.Release(_effect);
    }
    
}
