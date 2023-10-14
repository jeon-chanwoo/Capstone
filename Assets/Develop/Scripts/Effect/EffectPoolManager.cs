using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EffectPoolManager : SingletonBehaviour<EffectPoolManager>
{
    [SerializeField] private List<Effect> _effects = new List<Effect>();

    private Dictionary<string, ObjectPool<Effect>> _poolByName = new Dictionary<string, ObjectPool<Effect>>();

    public Effect Get(string name)
    {
        if(!_poolByName.ContainsKey(name))
        {
            var effectPrefab = GetPrefab(name);

            if(effectPrefab != null)
            {
                _poolByName.Add(name, new ObjectPool<Effect>(
                    () => CreateObject(effectPrefab),
                    GetObject,
                    ReleaseObject,
                    DestroyObject
                ));
            }
        }

        if(_poolByName.ContainsKey(name))
        {
            return _poolByName[name].Get();
        }
        return null;
    }

    public void Release(Effect effect)
    {
        var name = effect.name;

        if(_poolByName.ContainsKey(name))
        {
            _poolByName[name].Release(effect);
        }
        else
        {
            Destroy(effect.gameObject);
        }
    }

    public void Release(Effect effect, float delay)
    {
        StartCoroutine(ReleaseCoroutine(effect, delay));
    }

    private IEnumerator ReleaseCoroutine(Effect effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        Release(effect);
    }

    private Effect GetPrefab(string name)
    {
        return _effects.Find(e => e.name == name);
    }

    private Effect CreateObject(Effect effect)
    {
        return Instantiate(effect);
    }

    private void GetObject(Effect effect)
    {
        effect.gameObject.SetActive(true);
    }

    private void ReleaseObject(Effect effect)
    {
        effect.gameObject.SetActive(false);
        effect.transform.SetParent(transform);
    }

    private void DestroyObject(Effect effect)
    {
        Destroy(effect.gameObject);
    }
}
