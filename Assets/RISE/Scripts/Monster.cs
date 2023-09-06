using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.AI;

/*
 *가속도를 높게 속도를 낮게
 */
public abstract class Monster : MonoBehaviour
{
    public int Monster_Name;    // 몬스터 종류
    public int Monster_MaxHealth;   // 몬스터 최대 체력
    public int Monster_CurHealth;   // 몬스터 현재 체력
    public int Monster_AD;  // 몬스터 공격력
    public float Monster_Distance;  // 몬스터와 플레이어 사이 거리
    public float Monster_MSpeed;    // 몬스터 이동 속도
    public float Monster_ASpeed;    // 몬스터 공격 속도
    public float Monster_ADelay;    // 몬스터 공격 딜레이
    public float Monster_TraceDist; // 몬스터 인식 범위
    public float Monster_AttackRange;   // 몬스터 공격 범위(Raycast에서 사용)
    public State Monster_State; // 몬스터 상태 플래그

    public bool Monster_isAttak;    // 몬스터 공격 상태 플래그
    public bool Monster_isChase;    // 몬스터 추적 상태 플래그
    public enum State   // 몬스터 상태 플래그 종류
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

    public Transform Player_Transform;  // 플레이어 위치

    Color Monster_OriginColor;
    Ray M_Ray;
    RaycastHit forwordcheck;    // 전방 충돌 감지

    /*
     * 필요한 기능 : 추적, 공격, 피격, 사망
     * 몬스터가 플레이어의 위치로 이동(추적)
     * 몬스터가 플레이어를 공격 ( 근접, 원거리 등 나누어 구현하는 것이 좋을듯)
     * 몬스터가 플레이어에게 피격
     * 몬스터가 사망
     * overlap 함수로 근처 플레이어를 탐지
     */
    private void Awake()    // GetComponent 사용해서 초기화
    {
        Monster_Rigid = GetComponent<Rigidbody>();
        Monster_Animator = GetComponent<Animator>();
        Monster_BoxCollider = GetComponent<BoxCollider>();
        Monster_Material = GetComponentInChildren<SkinnedMeshRenderer>().material;
        Monster_Nav = GetComponent<NavMeshAgent>();
        Monster_OriginColor = Monster_Material.color;
        Monster_Nav.updateRotation = false; // 몬스터 회전 직접구현하기 때문에 navmeshagent의 회전off
        Monster_State = State.Idle;
    }

    public void Lookrotation()  // 몬스터 회전
    {
        if (Monster_State == State.Trace)
        {
            Vector3 lookratation = Monster_Nav.steeringTarget - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookratation), 0.1f);
        }
    }
    public void setting()   // 몬스터 초기화(추후 몬스터 딕셔너리 등 만들어서 종류별 값 저장시 사용)
    {

    }

    public bool Detect()    // 몬스터부터 구의 범위 내에 "Player" tag인 콜라이더가 들어오면 true 반환
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, Monster_TraceDist); // 몬스터 주변으로 구의 범위 내에 들어온 콜라이더 목록
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

    public void ChaseStart()    // 몬스터가 추격 시작
    {
        Monster_State = State.Trace;
        Monster_Nav.isStopped = false;
        Monster_Nav.updatePosition = true;
        FreezeVelocity();
        Monster_Animator.SetBool("isMove", true);
        Monster_Nav.SetDestination(Player_Transform.position);
    }

    public void ChaseStop()    // 몬스터가 추격 종료
    {
        Monster_State = State.Idle;
        Monster_Nav.isStopped = true;
        Monster_Nav.updatePosition = false;
        Monster_Nav.velocity = Vector3.zero;
        Monster_Animator.SetBool("isMove", false);
    }

    public void FreezeVelocity()   // 몬스터가 추적 중 플레이어와 물리 충돌시 플레이어에게 다가가지 못하는 현상 해결
    {
        Monster_Rigid.velocity = Vector3.zero;
        Monster_Rigid.angularVelocity = Vector3.zero;
    }

    public void Die()  // 몬스터 사망
    {
        Monster_Material.color = Color.gray;
        Monster_Animator.SetTrigger("isDie");
        gameObject.layer = 9;
        //reactVec = reactVec.normalized;
        //reactVec += Vector3.up;
        //Monster_Rigid.AddForce(reactVec*2, ForceMode.Impulse);
        Destroy(gameObject, 2); // 2초 뒤 사라짐
    }

    public virtual bool PlayerSearch()  // 몬스터에서 레이를 발사해 전방 거리 플레이어가 있는지 감지
    {
        Ray M_Ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(M_Ray, Monster_AttackRange, 1<<6))   // 플레이어 layer 가 6
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Attack()    // 몬스터가 공격
    {
        Debug.Log(name + " Monster Attack");
        Monster_Animator.SetTrigger("isAttack");
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 7) // layer 7은 player weapon 이다. 
        {
            // 무기 태그를 활용해서 공격 구분
            Weapon P_weapon = other.GetComponent<Weapon>();
            Monster_CurHealth -= P_weapon.damage;
            StartCoroutine(OnHit());
        }
    }

    IEnumerator OnHit() // 피격 구현
    {
        Monster_Material.color = Color.red;
        Monster_Animator.SetTrigger("isHit");
        yield return new WaitForSeconds(0.2f);  // 색상변경 유지시간 0.2f
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
