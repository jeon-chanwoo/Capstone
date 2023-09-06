using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

/*
 *���ӵ��� ���� �ӵ��� ����
 */
public abstract class Monster : MonoBehaviour
{
    public int Monster_Name;    // ���� ����
    public int Monster_MaxHealth;   // ���� �ִ� ü��
    public int Monster_CurHealth;   // ���� ���� ü��
    public int Monster_AD;  // ���� ���ݷ�
    public float Monster_Distance;  // ���Ϳ� �÷��̾� ���� �Ÿ�
    public float Monster_MSpeed;    // ���� �̵� �ӵ�
    public float Monster_ASpeed;    // ���� ���� �ӵ�
    public float Monster_ADelay;    // ���� ���� ������
    public float Monster_TraceDist; // ���� �ν� ����
    public float Monster_AttackRange;   // ���� ���� ����(Raycast���� ���)
    public State Monster_State; // ���� ���� �÷���

    public bool Monster_isAttak;    // ���� ���� ���� �÷���
    public bool Monster_isChase;    // ���� ���� ���� �÷���
    public enum State   // ���� ���� �÷��� ����
    {
        Idle,
        Trace,
        Attack,
        Die
    }

    Rigidbody Monster_Rigid;
    Animator Monster_Animator;
    BoxCollider Monster_BoxCollider;
    Material Monster_Material;
    NavMeshAgent Monster_Nav;

    public Transform Player_Transform;  // �÷��̾� ��ġ

    Color Monster_OriginColor;
    Ray M_Ray;
    RaycastHit forwordcheck;    // ���� �浹 ����

    /*
     * �ʿ��� ��� : ����, ����, �ǰ�, ���
     * ���Ͱ� �÷��̾��� ��ġ�� �̵�(����)
     * ���Ͱ� �÷��̾ ���� ( ����, ���Ÿ� �� ������ �����ϴ� ���� ������)
     * ���Ͱ� �÷��̾�� �ǰ�
     * ���Ͱ� ���
     * overlap �Լ��� ��ó �÷��̾ Ž��
     */
    private void Awake()    // GetComponent ����ؼ� �ʱ�ȭ
    {
        Monster_Rigid = GetComponent<Rigidbody>();
        Monster_Animator = GetComponent<Animator>();
        Monster_BoxCollider = GetComponent<BoxCollider>();
        Monster_Material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        Monster_Nav = GetComponent<NavMeshAgent>();
        Monster_OriginColor = Monster_Material.color;
        Monster_Nav.updateRotation = false; // ���� ȸ�� ���������ϱ� ������ navmeshagent�� ȸ��off
        Monster_State = State.Idle;
    }

    public void Lookrotation()  // ���� ȸ��
    {
        if (Monster_State == State.Trace)
        {
            Vector3 lookratation = Monster_Nav.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookratation), 0.1f);
        }
    }
    public void setting()   // ���� �ʱ�ȭ(���� ���� ��ųʸ� �� ���� ������ �� ����� ���)
    {

    }

    public bool Detect()    // ���ͺ��� ���� ���� ���� "Player" tag�� �ݶ��̴��� ������ true ��ȯ
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, Monster_TraceDist); // ���� �ֺ����� ���� ���� ���� ���� �ݶ��̴� ���
        if(cols.Length > 0 )
        {
            foreach(Collider col in cols)
            {
                if (col.tag == "Enermy") continue;
                else if(col.tag == "Player")
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void ChaseStart()    // ���Ͱ� �߰� ����
    {
        Monster_State = State.Trace;
        Monster_Nav.isStopped = false;
        Monster_Nav.updatePosition = true;
        FreezeVelocity();
        Monster_Animator.SetBool("isMove", true);
        Monster_Nav.SetDestination(Player_Transform.position);
    }

    public void ChaseStop()    // ���Ͱ� �߰� ����
    {
        Monster_State = State.Idle;
        Monster_Nav.isStopped = true;
        Monster_Nav.updatePosition = false;
        Monster_Nav.velocity = Vector3.zero;
        Monster_Animator.SetBool("isMove", false);
    }

    public void FreezeVelocity()   // ���Ͱ� ���� �� �÷��̾�� ���� �浹�� �÷��̾�� �ٰ����� ���ϴ� ���� �ذ�
    {
        Monster_Rigid.velocity = Vector3.zero;
        Monster_Rigid.angularVelocity = Vector3.zero;
    }

    public void Die()  // ���� ���
    {
        Monster_Material.color = Color.gray;
        Monster_Animator.SetTrigger("isDie");
        gameObject.layer = 9;
        //reactVec = reactVec.normalized;
        //reactVec += Vector3.up;
        //Monster_Rigid.AddForce(reactVec*2, ForceMode.Impulse);
        Destroy(gameObject, 2); // 2�� �� �����
    }

    public virtual bool PlayerSearch()  // ���Ϳ��� ���̸� �߻��� ���� �Ÿ� �÷��̾ �ִ��� ����
    {
        Ray M_Ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(M_Ray, Monster_AttackRange, 1<<6))   // �÷��̾� layer �� 6
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Attack()    // ���Ͱ� ����
    {
        Debug.Log(name + " Monster Attack");
        Monster_Animator.SetTrigger("isAttack");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7) // layer 7�� player weapon �̴�. 
        {
            // ���� �±׸� Ȱ���ؼ� ���� ����
            Weapon P_weapon = other.GetComponent<Weapon>();
            Monster_CurHealth -= P_weapon.damage;
            StartCoroutine(OnHit());
        }
    }

    IEnumerator OnHit() // �ǰ� ����
    {
        Monster_Material.color = Color.red;
        Monster_Animator.SetTrigger("isHit");
        yield return new WaitForSeconds(0.2f);  // ���󺯰� �����ð� 0.2f
        if(Monster_CurHealth > 0)
        {
            Monster_Material.color = Monster_OriginColor;
            yield return null;
        }
        else
        {
            Monster_State = State.Die;
        }
    }
    
}
