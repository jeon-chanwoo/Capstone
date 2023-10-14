using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : Effect
{
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public override void Play()
    {
        _particleSystem.Play();
    }
}
