using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Map1BossRoom : MonoBehaviour
{
    [System.Serializable]
    public struct BossData
    {
        public MonsterBase bossPrefab;
        public Vector3 position;
        public Vector3 rotation;
    }

    public int stageIndex;

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private CinematicManager _cinematicManager;

    [Header("Trigger")]
    [SerializeField] private Trigger _enterTrigger;
    [SerializeField] private Trigger _nextLevelTrigger;

    [Header("Common")]
    [SerializeField] private List<Animator> _frontDoors;
    [SerializeField] private List<Animator> _exitDoors;
    [SerializeField] private BackGroundMusic _backgroundMusic;

    [Header("Boss")]
    [SerializeField] private List<BossData> _bosses;

    [Header("Teleport")]
    [SerializeField] private GameObject _nextLevelTeleport;

    [Header("UI")]
    [SerializeField] private MonsterHpSlider _monsterHpSlider;

    private MonsterBase _spawnedBoss;
    private AsyncOperation _nextSceneOperation;

    private void OnEnable()
    {
        //_spawnTrigger.OnFiredTrigger += OnFiredSpawnTrigger;
        _enterTrigger.OnFiredTrigger += OnEnteredBossRoom;
        _nextLevelTrigger.OnFiredTrigger += OnEnteredNextLevel;
    }

    private void OnDisable()
    {
        //_spawnTrigger.OnFiredTrigger -= OnFiredSpawnTrigger;
        _enterTrigger.OnFiredTrigger -= OnEnteredBossRoom;
        _nextLevelTrigger.OnFiredTrigger -= OnEnteredNextLevel;
    }

    public MonsterBase SpawnBoss(int stageIndex)
    {
        var bossData = _bosses[stageIndex];

        _spawnedBoss = Instantiate(bossData.bossPrefab, bossData.position, Quaternion.Euler(bossData.rotation));
        _spawnedBoss.enabled = false;

        _spawnedBoss.OnBossDead += OnBossDead;

        return _spawnedBoss;
    }

    public void SpawnAndPlayCinematic()
    {
        var monster = SpawnBoss(stageIndex++);
        _monsterHpSlider.RegisterMonster(monster);

        _cinematicManager.Play(CinematicManager.CinematicType.DoorOpen, 2);
    }

    private void OnEnteredBossRoom()
    {
        PlayAnimators(_frontDoors, "close");
        PlayAnimators(_exitDoors, "close");

        _spawnedBoss.enabled = true;

        _backgroundMusic.PlayBossMusic();

        _monsterHpSlider.Show();
    }

    private void OnEnteredNextLevel()
    {
        float hp = _playerController._hp;
        float mp = _playerController._mp;

        PlayerPrefs.SetFloat("hp", hp);
        PlayerPrefs.SetFloat("mp", mp);

        _nextSceneOperation.allowSceneActivation = true;
    }

    private void OnBossDead(MonsterBase monster)
    {
        if(monster.GetComponent<MonsterThree>() != null)
        {
            _nextLevelTeleport.SetActive(true);

            _nextSceneOperation = SceneManager.LoadSceneAsync("FinalStage");
            _nextSceneOperation.allowSceneActivation = false;
        }

        _monsterHpSlider.Hide();
    }

    private void PlayAnimators(List<Animator> animators, string triggerName)
    {
        foreach(var animator in animators)
        {
            animator.SetTrigger(triggerName);
        }
    }
}
