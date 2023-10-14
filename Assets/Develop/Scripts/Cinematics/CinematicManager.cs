using Sirenix.OdinInspector;
using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicManager: MonoBehaviour
{
    public enum CinematicType
    {
        DoorOpen
    }

    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private Camera _cinematicCamera;

    [SerializeField] private Animator _doorLeft;
    [SerializeField] private Animator _doorRight;

    [Header("Door Open")]
    [SerializeField] private GameObject _doorOpen;
    [SerializeField] private PlayableDirector _timeline;

    private void OnEnable()
    {
        _timeline.stopped += OnTimelineStopped;
    }

    private void OnDisable()
    {
        _timeline.stopped -= OnTimelineStopped;
    }

    public void Play(CinematicType type, float delay)
    {
        StartCoroutine(PlayCoroutine(type, delay));
    }

    private IEnumerator PlayCoroutine(CinematicType type, float delay)
    {
        _playerController.isInteractable = false;

        yield return new WaitForSeconds(delay);

        Play(type);
    }

    public void Play(CinematicType type)
    {
        switch(type)
        {
            case CinematicType.DoorOpen:
                _playerController.isInteractable = false;

                _doorOpen.SetActive(true);
                _gameUI.SetActive(false);
                _cinematicCamera.gameObject.SetActive(true);

                _doorLeft.SetTrigger("open");
                _doorRight.SetTrigger("open");

                _timeline.gameObject.SetActive(true);
                _timeline.Play();
                break;
        }
    }

    private void OnTimelineStopped(PlayableDirector timeline)
    {
        _playerController.isInteractable = true;
        _doorOpen.gameObject.SetActive(false);
        _gameUI.SetActive(true);
        _cinematicCamera.gameObject.SetActive(false);
        timeline.gameObject.SetActive(false);
    }

    [Button]
    public void TestDoorOpen()
    {
        Play(CinematicType.DoorOpen);
    }
}
