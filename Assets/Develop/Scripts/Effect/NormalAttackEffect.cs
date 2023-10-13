using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttackEffect: Effect
{
    private ParticleSystem _particleSystem;
    private PlayerController _playerController;


    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
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
