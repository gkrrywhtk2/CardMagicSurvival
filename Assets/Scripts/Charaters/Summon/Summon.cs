using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Summon : MonoBehaviour
{
/********************************************************************************************

    [Header("Connect")]
    SummonScaner scaner;
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    public Transform leftAttackPoint;//왼쪽 공격시 타격 위치
    public Transform rightAttackPoint;
    //corutine 루프
    public Coroutine attackCorutine;

    [Header("Movement Settings")]
    public float moveSpeed = 1f; // 이동 속도
    public float damage;//공격시 데미지
    public float attackCoolTime;//공격 쿨타임
    public float duration;//소환 지속 시간
    private bool isAttacking;



    private void Awake() {
        scaner = GetComponentInChildren<SummonScaner>();
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        Init();
    }

    private void FixedUpdate() {
        if(scaner.nearestTarget != null && scaner.attackTarget == null){
            MoveTowards(scaner.nearestTarget.transform.position);
        }
        else if(scaner.nearestTarget != null && scaner.attackTarget != null){
            if(!isAttacking)
                StartAttack();
        }
        else{
            StopMovement();
        }
      
    }

      private void MoveToPlayer()
    {
    
        Vector2 moveVec = scaner.moveTarget.position - rb.position;
        Vector2 nextVec = moveVec.normalized * (moveSpeed + 2) * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + nextVec);
        //rigid.velocity = Vector2.zero;
    }

    public void Init(){
        duration = 10;
        damage = 10;
        attackCoolTime = 5;
    }

    public void StartAttack() {
    if (attackCorutine == null) {
        attackCorutine = StartCoroutine(GolemAttack(scaner.attackTarget.transform.position));
    }
    }

     private void MoveTowards(Vector2 targetPosition)
    {
       
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;

        //스프라이트 방향 설정
        sprite.flipX = direction.x < 0;

          // 이동 애니메이션 실행
        anim.SetBool("isMoving", true);
        
    }

    private void StopMovement(){
       
    rb.linearVelocity = Vector2.zero;
    anim.SetBool("isMoving", false);
    }

        IEnumerator GolemAttack(Vector2 attackTarget){

            isAttacking = true;
            StopMovement();
            yield return new WaitForSeconds(1f);//초기 공격 대기
            anim.SetTrigger("Attack");
        int poolingNum = 6;
        GameObject attackBox = GameManager.instance.effectPoolManager.Get(poolingNum);
        bullet attack = attackBox.GetComponent<bullet>();



        //위치 지정
        Vector2 direction = (attackTarget - (Vector2)transform.position).normalized;
        sprite.flipX = direction.x < 0;
        if(sprite.flipX == true){
            attackBox.GetComponent<Transform>().transform.position = leftAttackPoint.transform.position;
        }else{
            attackBox.GetComponent<Transform>().transform.position = rightAttackPoint.transform.position;
        }


        //공격 초기화
        int effect_Number = -1;
        int per = 0;//관통 현재 0
        float bulletspeed = 0;
        Vector3 dir = Vector3.zero;
        global::bullet.bulletType type = global::bullet.bulletType.placement;
        attack.GetComponent<bullet>().Init(damage, per, bulletspeed, dir,effect_Number,type);

        //공격 쿨타임 동안 공격 중지
        yield return new WaitForSeconds(attackCoolTime);
        isAttacking = false;
        attackCorutine = null;
    }
**/

    
}
