using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBossDance : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        int index = Random.Range(1, 31);
        _animator.Play("AA_Club_Dance_Moves_Type" + index.ToString("D2"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
