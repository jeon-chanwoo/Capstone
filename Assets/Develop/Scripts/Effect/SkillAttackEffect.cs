using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttackEffect : Effect
{
    [SerializeField] private AudioClip _audioClip;

    private ParticleSystem _particleSystem;
    private PlayerController _playerController;

    private AudioSource _audioSource;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if(_audioClip != null)
        {
            _audioSource.PlayOneShot(_audioClip);
        }
    }

    public void SetPlayer(PlayerController player)
    {
        _playerController = player;

        transform.position = player.transform.TransformPoint(Vector3.forward);
        transform.rotation = player.transform.rotation;
    }

    public override void Play()
    {
        _particleSystem.Play();
    }
}
