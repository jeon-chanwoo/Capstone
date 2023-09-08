using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Mimic : MonoBehaviour
{
    public int M_MaxHealth; // 몬스터 최대 체력
    public int M_CurHealth; // 몬스터 현재 체력
    public Transform M_Target;  // 몬스터 공격 대상
    public bool isAttack = false;   // 몬스터 공격 상태
    public bool isHit;  // 몬스터 피격 상태
    public bool isChase;    // 몬스터 어그로
    public int M_AD;    // 몬스터 공격력
    public float M_Distance;    // 플레이어와 몬스터 사이의 거리
    public float M_AttackDelay; // 몬스터 공격 딜레이 (공격과 공격 사이 걸리는 시간)

    Rigidbody M_Rigid;  // 몬스터 Rigidbody
    Animator M_Ani; // 몬스터 애니메이터
    BoxCollider M_BoxCollider;  // 몬스터 피격 범위
    Material M_Mat; // 몬스터 Material(몬스터가 피격시 몬스터의 색상변경을 위해)
    NavMeshAgent M_Nav; // 몬스터 Navi
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

    void ChaseStart()   // 추적 시작
    {
        isChase = true;
        M_Nav.isStopped = false;
        M_Nav.updatePosition = true;
        M_Ani.SetBool("isMove", true);
        M_Nav.SetDestination(M_Target.position);
    }

    void ChaseEnd()   // 추적 종료
    {
        isChase = false;
        M_Nav.isStopped = true;
        M_Nav.updatePosition = false;
        M_Nav.velocity = Vector3.zero;
        M_Ani.SetBool("isMove", false);
    }

    private void FreezeVelocity()   // 추적 중 플레이어와 물리 충돌시 플레이어에게 다가가지 못하는 현상 해결
    {
        if(isChase)
        {
            M_Rigid.velocity = Vector3.zero;
            M_Rigid.angularVelocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 몬스터 앞 방향의 공격 지역 계산 - 해당 지역에 플레이어 들어올 경우 애니메이션 재생(공격 딜레이가 아닐 경우) - 콜라이더 온 - 데미지 계산 - 콜라이더 오프
        if (other.tag == "THS")
        {
            Player_Weapon weapon = other.GetComponent<Player_Weapon>();
            M_CurHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnHit(reactVec));
        }
    }

    private IEnumerator OnHit(Vector3 reactVec) // 피격시 호출
    {
        M_Mat.color = Color.red;    // 몬스터 색상을 빨강으로 변경
        M_Ani.SetTrigger("isHit");  // 몬스터 피격 애니메이션 트리거

        if(M_CurHealth > 0) // 몬스터가 살아있다면
        {
            yield return new WaitForSeconds(0.2f);  // 색상 변경 유지 시간
            M_Mat.color = M_OriginColor;    // 몬스터 색상을 원래 색상으로 변경
        }
        else
        {
            ChaseEnd();
            M_Nav.enabled= false;
            M_Mat.color = Color.gray;   // 몬스터 색상을 회색으로 변경
            M_Ani.SetTrigger("isDie");  // 몬스터 죽는 애니메이션
            gameObject.layer = 9;   // 죽은 상태 레이어로 변경(물리충돌방지)
            
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            M_Rigid.AddForce(reactVec * 3, ForceMode.Impulse);  // 맞은 방향에서 튕겨 나옴
            Destroy(gameObject, 2);   // 2초 뒤 사라짐
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