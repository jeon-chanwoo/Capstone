using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Mimic : MonoBehaviour
{
    public int M_MaxHealth; // ���� �ִ� ü��
    public int M_CurHealth; // ���� ���� ü��
    public Transform M_Target;  // ���� ���� ���
    public bool isAttack = false;   // ���� ���� ����
    public bool isHit;  // ���� �ǰ� ����
    public bool isChase;    // ���� ��׷�
    public int M_AD;    // ���� ���ݷ�
    public float M_Distance;    // �÷��̾�� ���� ������ �Ÿ�
    public float M_AttackDelay; // ���� ���� ������ (���ݰ� ���� ���� �ɸ��� �ð�)

    Rigidbody M_Rigid;  // ���� Rigidbody
    Animator M_Ani; // ���� �ִϸ�����
    BoxCollider M_BoxCollider;  // ���� �ǰ� ����
    Material M_Mat; // ���� Material(���Ͱ� �ǰݽ� ������ ���󺯰��� ����)
    NavMeshAgent M_Nav; // ���� Navi
    Monster_Attack M_weapon;

    Color M_OriginColor;

    private void Awake()
    {
        M_Rigid = GetComponent<Rigidbody>();
        M_Ani = GetComponent<Animator>();
        M_Mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        M_OriginColor = M_Mat.color;
        M_Nav = GetComponent<NavMeshAgent>();
        M_BoxCollider = GetComponent<BoxCollider>();
        M_weapon = GetComponentInChildren<Monster_Attack>();
    }

    private void Update()
    {
        M_Distance = Vector3.Distance(M_Target.position, this.transform.position);
        M_AttackDelay += Time.deltaTime;
        if(M_Distance > 6.0f && !isAttack)
        {
            ChaseEnd();
        }
       else if(M_Distance < 6.0f && M_Distance > 2.0f && !isAttack)
        {
            ChaseStart();
        }
        else if(!isAttack && M_AttackDelay <= 0.8f && M_Distance < 2.0f)
        {
            M_Attack();
        }
    }

    private void FixedUpdate()
    {
        FreezeVelocity();
    }

    void ChaseStart()   // ���� ����
    {
        isChase = true;
        M_Nav.isStopped = false;
        M_Nav.updatePosition = true;
        M_Ani.SetBool("isMove", true);
        M_Nav.SetDestination(M_Target.position);
    }

    void ChaseEnd()   // ���� ����
    {
        isChase = false;
        M_Nav.isStopped = true;
        M_Nav.updatePosition = false;
        M_Nav.velocity = Vector3.zero;
        M_Ani.SetBool("isMove", false);
    }

    private void FreezeVelocity()   // ���� �� �÷��̾�� ���� �浹�� �÷��̾�� �ٰ����� ���ϴ� ���� �ذ�
    {
        if(isChase)
        {
            M_Rigid.velocity = Vector3.zero;
            M_Rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���� �� ������ ���� ���� ��� - �ش� ������ �÷��̾� ���� ��� �ִϸ��̼� ���(���� �����̰� �ƴ� ���) - �ݶ��̴� �� - ������ ��� - �ݶ��̴� ����
        if (other.tag == "THS")
        {
            Player_Weapon weapon = other.GetComponent<Player_Weapon>();
            M_CurHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnHit(reactVec));
        }
    }

    private IEnumerator OnHit(Vector3 reactVec) // �ǰݽ� ȣ��
    {
        M_Mat.color = Color.red;    // ���� ������ �������� ����
        M_Ani.SetTrigger("isHit");  // ���� �ǰ� �ִϸ��̼� Ʈ����

        if(M_CurHealth > 0) // ���Ͱ� ����ִٸ�
        {
            yield return new WaitForSeconds(0.2f);  // ���� ���� ���� �ð�
            M_Mat.color = M_OriginColor;    // ���� ������ ���� �������� ����
        }
        else
        {
            ChaseEnd();
            M_Nav.enabled= false;
            M_Mat.color = Color.gray;   // ���� ������ ȸ������ ����
            M_Ani.SetTrigger("isDie");  // ���� �״� �ִϸ��̼�
            gameObject.layer = 9;   // ���� ���� ���̾�� ����(�����浹����)
            
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            M_Rigid.AddForce(reactVec * 3, ForceMode.Impulse);  // ���� ���⿡�� ƨ�� ����
            Destroy(gameObject, 2);   // 2�� �� �����
        }
    }
    
    void M_Attack()
    {
        isAttack = true;
        ChaseEnd();
        M_Ani.SetTrigger("isAttack");
        M_weapon.use();
        M_AttackDelay = 0;
    }
}