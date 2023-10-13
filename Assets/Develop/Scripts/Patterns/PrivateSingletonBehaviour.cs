using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivateSingletonBehaviour<T> : MonoBehaviour
    where T: Component
{
    private static T _instance = null;
    protected static T instance {
        get {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<T>();
            return _instance;
        }
    }
}
