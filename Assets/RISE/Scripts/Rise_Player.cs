using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float P_Speed;   // 플레이어 속도
    public int P_MaxHealth; // 플레이어 전체 체력
    public int P_CurHealth; // 플레이어 현재 체력

    float hAxis;    // 플레이어 가로이동
    float vAxis;    // 플레이어 세로이동

    bool fDown; // 플레이어 공격키
    
    bool isAttackReady = true;  // 플레이어 공격준비
    float AttackDelay= 0f;  //  플레이어 공격간격
    bool isHit;  // 플레이어 피격상태

    Vector3 P_moveVec;

    Rigidbody P_Rigid;
    Animator P_Animator;
    Player_Weapon Eq_Weapon;

    private void Awake()
    {
        P_Animator = GetComponent<Animator>();
        P_Rigid = GetComponentInChildren<Rigidbody>();
        Eq_Weapon = GetComponentInChildren<Player_Weapon>();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        fDown = Input.GetButton("Attack1");
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        P_Attack();
    }

    private void FixedUpdate()
    {
        FreezeRotation();
    }

    void Move()
    {
        P_moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if(!isAttackReady) {
            P_moveVec = Vector3.zero;
        }
        transform.position += P_moveVec * P_Speed * Time.deltaTime;

        P_Animator.SetBool("isMove", P_moveVec != Vector3.zero);
    }

    void Turn()
    {
        transform.LookAt(transform.position + P_moveVec);
    }

    private void FreezeRotation()   // 자동회전 방지
    {
        P_Rigid.angularVelocity = Vector3.zero;
    }

    void P_Attack()
    {
        AttackDelay += Time.deltaTime;
        isAttackReady = Eq_Weapon.rate <= AttackDelay;
        if (fDown && isAttackReady)
        {
            Eq_Weapon.use();
            P_Animator.SetTrigger("doAttack1");
            AttackDelay = 0;
        }
    }

    IEnumerator OnHit()
    {
        isHit = true;
        P_Animator.SetTrigger("isHit");
        yield return new WaitForSeconds(0.5f);
        isHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "EnermyAttack")
        {
            if (!isHit)
            {
                // Monster_Attack 과 상호작용
            }
        }
    }
}