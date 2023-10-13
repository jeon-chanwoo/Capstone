using MonsterLove.StateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FinalBoss: MonsterBase
{
    private enum State
    {
        Idle,
        Think,
        Move,
        Rotate,
        NormalAttack,
        Stamp,
        Dead
    }

    private class Driver
    {
        public StateEvent Update;
    }

    [Header("Common")]
    [SerializeField] private float _maxHp = 1000;

    [Header("Navigation")]
    [SerializeField] private float _attackableAngle;

    [Header("Normal Attack")]
    [SerializeField] private float _normalAttackPreDelay;
    [SerializeField] private float _normalAttackPunchDelay;
    [SerializeField] private float _normalAttackPostDelay;
    [SerializeField] private GameObject _punchTrigger;
    [SerializeField] private AudioClip _punchClip;

    [Header("Stamp Attack")]
    [SerializeField] private float _stampAttackPreDelay;
    [SerializeField] private float _stampAttackPostDelay;

    [Header("Sound")]
    [SerializeField] private AudioClip _hittedClip;

    private AudioSource _audioSource;
    private Animator _animator;
    private NavMeshAgent _agent;
    private StateMachine<State, Driver> _fsm;

    private float _currentHp;

    public override bool isDead => _currentHp <= 0;
    public override float maxHp => _maxHp;
    public override float currentHp => _currentHp;

    private GameObject _target;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        _fsm = new StateMachine<State, Driver>(this);
        _fsm.ChangeState(State.Idle);
    }

    private void Start()
    {
        _currentHp = _maxHp;

        _target = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        _fsm.Driver.Update.Invoke();
    }

    #region Idle State
    private void Idle_Enter()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        _fsm.ChangeState(State.Think);
    }
    #endregion

    #region Think State
    private void Think_Update()
    {
        if(IsAttackable())
        {
            ChooseAttackState();
        }
        else if(IsReached() && !IsAttackableRotation())
        {
            _fsm.ChangeState(State.Rotate);
        }
        else if(IsRechable())
        {
            _fsm.ChangeState(State.Move);
        }
        else
        {
            _fsm.ChangeState(State.Idle);
        }
    }

    private void ChooseAttackState()
    {
        if(Random.Range(0f, 1f) > 0.3f)
        {
            _fsm.ChangeState(State.NormalAttack);
        }
        else
        {
            _fsm.ChangeState(State.Stamp);
        }
    }
    #endregion

    #region Move State
    private void Move_Enter()
    {
        _agent.updatePosition = true;
        _agent.updateRotation = true;

        _animator.SetBool("walking", true);
    }

    private void Move_Exit()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;

        _animator.SetBool("walking", false);
    }

    private void Move_Update()
    {
        if(IsReached())
        {
            _fsm.ChangeState(State.Idle);
        }
        else if(IsRechable())
        {
            _agent.destination = _target.transform.position;
        }
        else
        {
            _fsm.ChangeState(State.Think);
        }
    }
    #endregion

    #region Rotate State
    private void Rotate_Enter()
    {
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    private void Rotate_Update()
    {
        if(IsReached() && !IsAttackableRotation())
        {
            transform.Rotate(Vector3.up, GetRotationToDestination() * _agent.angularSpeed * Time.deltaTime * 0.01f);
        }
        else
        {
            _fsm.ChangeState(State.Think);
        }
    }
    #endregion

    #region NormalAttack State
    private IEnumerator NormalAttack_Enter()
    {
        yield return new WaitForSeconds(_normalAttackPreDelay);

        _audioSource.PlayOneShot(_punchClip);
        _animator.SetTrigger("LeftPunch");

        yield return new WaitForSeconds(_normalAttackPunchDelay);

        _audioSource.PlayOneShot(_punchClip);
        _animator.SetTrigger("LeftPunch");

        yield return new WaitForSeconds(_normalAttackPunchDelay);

        _audioSource.PlayOneShot(_punchClip);
        _animator.SetTrigger("LeftPunch");

        yield return new WaitForSeconds(_normalAttackPunchDelay);

        _audioSource.PlayOneShot(_punchClip);
        _animator.SetTrigger("RightPunch");

        yield return new WaitForSeconds(_normalAttackPunchDelay);

        _audioSource.PlayOneShot(_punchClip);
        _animator.SetTrigger("RightPunch");

        yield return new WaitForSeconds(_normalAttackPostDelay);

        _fsm.ChangeState(State.Think);
    }
    #endregion

    #region Stamp State
    private IEnumerator Stamp_Enter()
    {
        yield return new WaitForSeconds(_stampAttackPreDelay);

        var position = transform.position;

        _animator.SetTrigger("Stamp");
        
        yield return new WaitForSeconds(2f);
        
        var effect = EffectPoolManager.instance.Get("Stamp");
        EffectPoolManager.instance.Release(effect, 2f);
        effect.transform.position = position;
        effect.Play();

        yield return new WaitForSeconds(_stampAttackPostDelay);

        _fsm.ChangeState(State.Think);
    }
    #endregion

    private void Dead_Enter()
    {
        //isDead = true;
        _animator.SetTrigger("Dead");

        Destroy(gameObject, 3f);

        OnBossDead?.Invoke(this);
    }

    public override void TakeDamage(float damageAmount)
    {
        if(_currentHp > 0)
        {
            _currentHp = Mathf.Max(0, _currentHp - damageAmount);
            OnDamaged?.Invoke(_currentHp, damageAmount);

            if(_currentHp <= 0)
            {
                _fsm.ChangeState(State.Dead, StateTransition.Overwrite);
            }
        }

        _animator.SetTrigger("Hitted");
        _audioSource.PlayOneShot(_hittedClip);
    }

    private bool IsAttackable()
    {
        return IsReached() && IsAttackableRotation();
    }

    private bool IsAttackableRotation()
    {
        return Mathf.Abs(GetRotationToDestination()) < _attackableAngle;
    }

    private float GetRotationToDestination()
    {
        var forward = transform.forward;
        var direction = (_target.transform.position - transform.position);

        forward.y = 0f;
        direction.y = 0f;

        forward.Normalize();
        direction.Normalize();

        var angleDiff = Quaternion.FromToRotation(forward, direction).eulerAngles.y;
        if(angleDiff > 180)
            angleDiff = angleDiff - 360f;

        return angleDiff;
    }

    private bool IsReached()
    {
        var distance = Vector3.Distance(transform.position, _target.transform.position);
        
        return distance <= _agent.stoppingDistance;
    }

    private bool IsRechable()
    {
        NavMeshPath path = new NavMeshPath();
        
        return _agent.CalculatePath(_target.transform.position, path);
    }

    public void Punch()
    {
        _punchTrigger.SetActive(true);
    }
}
