using System.Collections;
using UnityEngine;

public class EGolem : MonoBehaviour
{
    /**********************************************************************************************
   [Header("Detection Settings")]
    public float detectionRange = 5f; // 탐지 범위
    public float attackRange = 1f; // 공격 범위
    public LayerMask targetLayer; // 탐지할 타겟 레이어

    [Header("Attack Settings")]
    public float attackCooldown = 1f; // 공격 간격
    public int damage = 10; // 공격력
    public bool attackReady;// 몬스터와 충돌하면 공격 준비
    public Transform attackPointRight;
    public Transform attackPointLeft;

    [Header("Movement Settings")]
    public float moveSpeed = 1f; // 이동 속도

    private Transform nearestTarget; // 가장 가까운 타겟
    private bool isAttacking = false; // 공격 중인지 확인
    private float attackTimer = 0f; // 공격 쿨타임 타이머

    private Rigidbody2D rb;
    public SpriteRenderer sprite;
    public Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        // 가장 가까운 타겟 찾기
        nearestTarget = FindNearestTarget();

        if (nearestTarget != null)
        {
            if (attackReady == true && attackTimer == 0)
            {
                StartCoroutine(GolemAttack());
            }
            else if(attackReady == true && attackTimer != 0)
            {
            StopMovement();
           
            }
            else if(attackReady != true){
                MoveTowards(nearestTarget.transform.position);
            }
}
else
{
    StopMovement();
}

        // 공격 쿨타임 감소
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    private Transform FindNearestTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, detectionRange, targetLayer);
        Transform nearest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D target in targets)
        {
            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = target.transform;
            }
        }

        return nearest;
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

     private void StopMovement()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isMoving", false); // 이동 애니메이션 종료
    }
    private void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        // 타겟에 데미지 주기 (예: Health 컴포넌트)
        int effectNumber = 6;
        GameObject attackBox = GameManager.instance.effectPoolManager.Get(effectNumber);
        bullet attack = attackBox.GetComponent<bullet>();

        if(sprite.flipX == true){
            attackBox.GetComponent<Transform>().transform.position = attackPointLeft.transform.position;
        }else{
            attackBox.GetComponent<Transform>().transform.position = attackPointRight.transform.position;
        }

        //bullet 초기화
        float damage = 5;//임시
        int effect_Number = -1;
        int per = 0;//관통 현재 0
        float bulletspeed = 0;
        Vector3 dir = Vector3.zero;
         global::bullet.bulletType type = global::bullet.bulletType.bullet;
         attack.GetComponent<bullet>().Init(damage, per, bulletspeed, dir,effect_Number,type);
        

        anim.SetTrigger("Attack");

      
    }

    IEnumerator GolemAttack(){
        isAttacking = true;
        yield return new WaitForSeconds(attackCooldown);
        attackTimer = attackCooldown;

        int poolingNum = 6;
        GameObject attackBox = GameManager.instance.effectPoolManager.Get(poolingNum);
        bullet attack = attackBox.GetComponent<bullet>();

        //위치 지정
        if(sprite.flipX == true){
            attackBox.GetComponent<Transform>().transform.position = attackPointLeft.transform.position;
        }else{
            attackBox.GetComponent<Transform>().transform.position = attackPointRight.transform.position;
        }

        //bullet 초기화
        float damage = 5;//임시
        int effect_Number = -1;
        int per = 0;//관통 현재 0
        float bulletspeed = 0;
        Vector3 dir = Vector3.zero;
        global::bullet.bulletType type = global::bullet.bulletType.placement;
        attack.GetComponent<bullet>().Init(damage, per, bulletspeed, dir,effect_Number,type);
        

        anim.SetTrigger("Attack");
    }

   
    private void OnCollisionEnter2D(Collision2D other) {
         if(other.gameObject.CompareTag("Monster")){
            attackReady = true;
        }
     }

private void OnCollisionExit2D(Collision2D other) {
     if(other.gameObject.CompareTag("Monster")){
            attackReady = false;
        }
}
    
    public void EndAttacking(){
        isAttacking = false;
    }
    **/
}
