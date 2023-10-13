using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class FinalBossMockup : MonsterBase
{
    public Action<int, int> OnHittedMockup;

    [SerializeField] private List<string> _poseAnimationNames;
    [SerializeField] private List<Material> _faceMaterials;

    [SerializeField] private Renderer _faceRenderer;
    [SerializeField] private Animator _animator;

    public List<string> poseAnimationNames => _poseAnimationNames;
    public List<Material> faceMaterials => _faceMaterials;

    public int poseIndex { get; set; }
    public int faceIndex { get; set; }

    public override bool isDead => false;
    public override float maxHp => 0;
    public override float currentHp => 0;

    public void Punch()
    {
        _animator.Play("Punch");
    }

    public void SetPoseFace(int poseIndex, int faceIndex)
    {
        this.poseIndex = poseIndex;
        this.faceIndex = faceIndex;

        _animator.Play(_poseAnimationNames[poseIndex]);
        List<Material> materias = new List<Material>();
        
        _faceRenderer.GetSharedMaterials(materias);
        materias[1] = _faceMaterials[faceIndex];
        _faceRenderer.SetSharedMaterials(materias);
    }

    public override void TakeDamage(float damageAmount)
    {
        OnHittedMockup?.Invoke(poseIndex, faceIndex);
    }
}
