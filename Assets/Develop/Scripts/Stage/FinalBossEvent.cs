using MonsterLove.StateMachine;
using Sirenix.OdinInspector;
using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossEvent : MonoBehaviour
{
    private enum State
    {
        None,
        Preview,
        Finding,
        Correct,
        Incorrect
    }

    private class Driver
    {
        public StateEvent Update;
    }

    [SerializeField] private Transform _centerPoint;
    [SerializeField] private MonsterBase _bossMonster;
    [SerializeField] private PlayerController _player;
    [SerializeField] private Image _blackScreen;

    [SerializeField] private FinalBossMockup _bossMockupPrefab;
    
    private StateMachine<State, Driver> _fsm;

    private List<int> _poseIndexes;
    private List<int> _faceIndexes;

    private FinalBossMockup _previewMockup;
    private FinalBossMockup _correctMockup;
    private List<FinalBossMockup> _findingMockups = new List<FinalBossMockup>();

    private void Awake()
    {
        _fsm = new StateMachine<State, Driver>(this);
        _fsm.ChangeState(State.None);
    }

    private void OnEnable()
    {
        _bossMonster.OnDamaged += OnBossDamaged;
    }

    private void Update()
    {
        _fsm.Driver.Update.Invoke();
    }

    private void None_Enter()
    {
        _previewMockup = null;
        _correctMockup = null;

        foreach(var mockup in _findingMockups)
        {
            if(mockup != null)
            {
                Destroy(mockup.gameObject);
            }
        }

        _findingMockups.Clear();
    }

    private IEnumerator Preview_Enter()
    {
        _player.isInteractable = false;
        _bossMonster.gameObject.SetActive(false);

        _blackScreen.CrossFadeAlpha(1f, 0.5f, false);
        yield return new WaitForSeconds(0.5f);
        _blackScreen.CrossFadeAlpha(0f, 0.5f, false);

        MovePlayerToCenter();
        _player.isInteractable = true;

        _poseIndexes = GetRandomPoseIndexes();
        _faceIndexes = GetRandomFaceIndexes();

        var playerForward = _player.direction.transform.forward;
        _previewMockup = InstantiateMockup();
        _previewMockup.SetPoseFace(_poseIndexes[0], _faceIndexes[0]);

        _previewMockup.transform.position = _player.transform.position + playerForward * 3;
        _previewMockup.transform.rotation = Quaternion.LookRotation(-playerForward, Vector3.up);

        yield return new WaitForSeconds(3f);

        _fsm.ChangeState(State.Finding);
    }

    private void Preview_Exit()
    {
        Destroy(_previewMockup.gameObject);
    }

    private IEnumerator Finding_Enter()
    {
        _findingMockups = new List<FinalBossMockup>();

        for(int i = 0; i < _poseIndexes.Count; i++)
        {
            for(int j = 0; j < _faceIndexes.Count; j++)
            {
                var mockup = InstantiateMockup();
                mockup.SetPoseFace(_poseIndexes[i], _faceIndexes[j]);
                mockup.OnHittedMockup += OnHittedMockup;

                _findingMockups.Add(mockup);
            }
        }

        _correctMockup = _findingMockups.Find(e => e.poseIndex == _poseIndexes[0] && e.faceIndex == _faceIndexes[0]);

        _findingMockups.Shuffle();

        var positions = GetFindingPositions(_player.direction.transform.forward, 8);
        for(int i=0; i < positions.Count; i++)
        {
            var mockup = _findingMockups[i];
            var position = positions[i];

            mockup.transform.position = position;
            mockup.transform.rotation = Quaternion.LookRotation(_centerPoint.position - position, Vector3.up);
        }

        yield return new WaitForSeconds(5f);

        _fsm.ChangeState(State.Incorrect);
    }

    private IEnumerator Correct_Enter()
    {
        yield return new WaitForSeconds(1f);

        foreach(var mockup in _findingMockups)
        {
            mockup.gameObject.SetActive(false);
        }

        _bossMonster.TakeDamage(200);
        _bossMonster.transform.position = _player.transform.position + _player.direction.transform.forward * 5;
        _bossMonster.transform.rotation = Quaternion.LookRotation(_player.transform.position - _bossMonster.transform.position, Vector3.up);
        _bossMonster.gameObject.SetActive(true);

        var smoke = EffectPoolManager.instance.Get("Smoke");
        smoke.transform.position = _bossMonster.transform.position;
    }

    private IEnumerator Incorrect_Enter()
    {
        yield return new WaitForSeconds(1f);

        foreach(var mockup in _findingMockups)
        {
            if(mockup.poseIndex == _previewMockup.poseIndex && mockup.faceIndex == _previewMockup.faceIndex)
            {
                mockup.Punch();
            }
            else
            {
                mockup.gameObject.SetActive(false);
            }
        }

        yield return new WaitForSeconds(1f);

        _player.TakeDamage(40);

        _correctMockup.gameObject.SetActive(false);

        _bossMonster.transform.position = _correctMockup.transform.position;
        _bossMonster.transform.rotation = _correctMockup.transform.rotation;
        _bossMonster.gameObject.SetActive(true);

        Destroy(_correctMockup);

        _fsm.ChangeState(State.None);
    }

    private FinalBossMockup InstantiateMockup()
    {
        var mockup = Instantiate(_bossMockupPrefab);
        return mockup;
    }

    private void OnBossDamaged(float hp, float damage)
    {
        if(hp + damage > 900 && hp <= 900 ||
            hp + damage > 600 && hp <= 600 ||
            hp + damage > 300 && hp <= 300)
        {
            if(!_bossMonster.isDead)
            {
                _fsm.ChangeState(State.Preview);
            }
        }
    }

    [Button]
    public void StartEvent()
    {
        _fsm.ChangeState(State.Preview);
    }

    private List<Vector3> GetFindingPositions(Vector3 forward, float distance)
    {
        List<Vector3> positions = new List<Vector3>();

        positions.Add(Quaternion.Euler(0, 0, 0) * forward * distance + _centerPoint.transform.position);
        positions.Add(Quaternion.Euler(0, 90, 0) * forward * distance + _centerPoint.transform.position);
        positions.Add(Quaternion.Euler(0, 180, 0) * forward * distance + _centerPoint.transform.position);
        positions.Add(Quaternion.Euler(0, 270, 0) * forward * distance + _centerPoint.transform.position);

        return positions;
    }

    private List<int> GetRandomPoseIndexes()
    {
        var count = _bossMockupPrefab.poseAnimationNames.Count;

        int index = Random.Range(0, count);
        int index2 = (index + Random.Range(1, count - 1)) % count;

        return new List<int>() { index, index2 };
    }

    private List<int> GetRandomFaceIndexes()
    {
        var count = _bossMockupPrefab.faceMaterials.Count;

        int index = Random.Range(0, count);
        int index2 = (index + Random.Range(1, count - 1)) % count;

        return new List<int>() { index, index2 };
    }

    private void OnHittedMockup(int poseIndex, int faceIndex)
    {
        if(poseIndex == _correctMockup.poseIndex && faceIndex == _correctMockup.faceIndex)
        {
            _fsm.ChangeState(State.Correct, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
        else
        {
            _fsm.ChangeState(State.Incorrect, MonsterLove.StateMachine.StateTransition.Overwrite);
        }
    }

    private void MovePlayerToCenter()
    {
        var characterController = _player.GetComponent<CharacterController>();

        characterController.enabled = false;        

        _player.transform.position = _centerPoint.position;

        characterController.enabled = true;
    }
}
