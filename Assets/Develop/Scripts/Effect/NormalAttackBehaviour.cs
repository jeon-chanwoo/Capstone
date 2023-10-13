using Suntail;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class NormalAttackBehavior : StateMachineBehaviour
{
    [SerializeField] private string _effectName;
    [SerializeField] private Vector3 _positionOffset;
    [SerializeField] private Vector3 _rotationOffset;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerController = animator.GetComponent<PlayerController>();
        var direction = playerController.direction;

        var effect = EffectPoolManager.instance.Get(_effectName);
        effect.transform.position = direction.transform.TransformPoint(Vector3.forward + _positionOffset) + Vector3.up * 0.8f;
        effect.transform.rotation = direction.transform.rotation * Quaternion.Euler(_rotationOffset);
        effect.Play();

        EffectPoolManager.instance.Release(effect, 2f);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }

    override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
