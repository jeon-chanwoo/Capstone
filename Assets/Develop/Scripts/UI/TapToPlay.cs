using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToPlay : MonoBehaviour
{
    [SerializeField] private float _inputDelay = 3f;

    private float _startTime;
    private AsyncOperation _asyncOperation;

    void Start()
    {
        _asyncOperation = SceneManager.LoadSceneAsync("Dungeon");
        _asyncOperation.allowSceneActivation = false;

        _startTime = Time.time;
    }

    void Update()
    {
        if(_startTime + _inputDelay > Time.time)
            return;

        if(Input.GetMouseButtonUp(0))
        {
            _asyncOperation.allowSceneActivation = true;
        }
    }
}
