using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float P_Speed;   // �÷��̾� �ӵ�
    public int P_MaxHealth; // �÷��̾� ��ü ü��
    public int P_CurHealth; // �÷��̾� ���� ü��

    float hAxis;    // �÷��̾� �����̵�
    float vAxis;    // �÷��̾� �����̵�

    bool fDown; // �÷��̾� ����Ű
    
    bool isAttackReady = true;  // �÷��̾� �����غ�
    float AttackDelay= 0f;  //  �÷��̾� ���ݰ���
    bool isHit;  // �÷��̾� �ǰݻ���

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

    private void FreezeRotation()   // �ڵ�ȸ�� ����
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
                // Monster_Attack �� ��ȣ�ۿ�
            }
        }
    }
}