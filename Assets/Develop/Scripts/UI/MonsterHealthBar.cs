using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] private Slider _hpSlider1;
    [SerializeField] private Text _hpText1;
    [SerializeField] private MonsterBase monster;

    //private void Start()
    //{
    //    Transform hpBarTransform = GameObject.Find("MonsterHpBar").transform;
    //    _hpSlider1 = hpBarTransform.GetComponent<Slider>();
    //    _hpSlider1.minValue = 0;

    //    monster = GameObject.Find("Green1(Clone)").GetComponent<Monster>();
    //}

    private void UpdateUI()
    {
        _hpSlider1.maxValue = monster.maxHp;
        _hpSlider1.value = monster.currentHp;
        _hpText1.text = $"{monster.currentHp}/{monster.maxHp}";
    }

    private void Update()
    {
        UpdateUI();
    }
}
