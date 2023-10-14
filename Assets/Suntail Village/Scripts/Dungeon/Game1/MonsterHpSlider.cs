using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace Suntail
{
    public class MonsterHpSlider : MonoBehaviour
    {
        [SerializeField] private GameObject _sliderPanel;
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _text;

        private MonsterBase _monster;

        private void Start()
        {
            _slider.minValue = 0;
        }

        private void Update()
        {
            if(_monster != null)
            {
                _slider.maxValue = _monster.maxHp;
                _slider.value = _monster.currentHp;
                _text.text = $"{_monster.currentHp}/{_monster.maxHp}";
            }
        }

        public void RegisterMonster(MonsterBase monster)
        {
            _monster = monster;
        }

        public void Show()
        {
            _sliderPanel.SetActive(true);
        }

        public void Hide()
        {
            _sliderPanel.SetActive(false);
        }
    }
}