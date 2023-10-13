using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditPanel : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private float _speed;

    private void Start()
    {
        _scrollRect.verticalNormalizedPosition = 1f;

        StartCoroutine(RollCreditCoroutine());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Title");
        }
    }

    private IEnumerator RollCreditCoroutine()
    {
        yield return new WaitForSeconds(1f);

        var verticalValue = 1f;

        while(verticalValue > 0)
        {
            yield return null;

            verticalValue = verticalValue - _speed * Time.deltaTime;
            verticalValue = Mathf.Clamp01(verticalValue);

            _scrollRect.verticalNormalizedPosition = verticalValue;
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Title");
    }
}
