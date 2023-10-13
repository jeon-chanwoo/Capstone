using MonsterLove.StateMachine;
using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossRoom: MonoBehaviour
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Transform _stageCenterPoint;
    [SerializeField] private InteractionTrigger _statue;
    [SerializeField] private MonsterBase _bossMonster;
    [SerializeField] private MonsterHpSlider _monsterHpSlider;
    [SerializeField] private GameObject _playerUI;
    [SerializeField] private Image _blackScreen;

    [SerializeField] private Trigger _fallingTrigger;

    [Header("End")]
    [SerializeField] private Trigger _clearTrigger;
    [SerializeField] private GameObject _creditObject;

    private BackGroundMusic _backgroundMusic;

    private void Awake()
    {
        _backgroundMusic = _player.GetComponentInChildren<BackGroundMusic>();
    }

    private IEnumerator Start()
    {
        _statue.OnFiredTrigger += OnInteractWithStatue;
        _clearTrigger.OnFiredTrigger += OnClearFired;
        _bossMonster.OnBossDead += OnBossDead;
        _fallingTrigger.OnFiredTrigger += OnPlayerFall;

        _bossMonster.gameObject.SetActive(false);
        _player.isInteractable = false;

        yield return new WaitForSeconds(5f);

        _player.isInteractable = true;
    }

    private void OnInteractWithStatue()
    {
        _statue.gameObject.SetActive(false);

        var effect = EffectPoolManager.instance.Get("Smoke");
        effect.transform.position = _bossMonster.transform.position;
        _bossMonster.gameObject.SetActive(true);

        _monsterHpSlider.RegisterMonster(_bossMonster);
        _monsterHpSlider.Show();

        _backgroundMusic.PlayBossMusic(true);
    }

    private void OnBossDead(MonsterBase monster)
    {
        _monsterHpSlider.Hide();
        _clearTrigger.gameObject.SetActive(true);

        _backgroundMusic.StopBossMusic();
    }

    private void OnClearFired()
    {
        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator EndingCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        _blackScreen.CrossFadeAlpha(1f, 0.2f, false);
        yield return new WaitForSeconds(0.2f);

        _player.GetComponent<AudioSource>().enabled = false;

        _player.isInteractable = false;
        _playerUI.SetActive(false);

        _backgroundMusic.PlayEndingMusic();

        _blackScreen.CrossFadeAlpha(0f, 0.2f, false);

        _creditObject.SetActive(true);
    }

    private void OnPlayerFall()
    {
        MovePlayerToCenter();
        _player.TakeDamage(1000);
    }

    private void MovePlayerToCenter()
    {
        var characterController = _player.GetComponent<CharacterController>();

        characterController.enabled = false;

        _player.transform.position = _stageCenterPoint.position;

        characterController.enabled = true;
    }
}
