using UnityEngine;

public class EGolem : MonoBehaviour
{
   [Header("Detection Settings")]
    public float detectionRange = 10f; // 탐지 범위
    public float attackRange = 2f; // 공격 범위
    public LayerMask targetLayer; // 탐지할 타겟 레이어

    [Header("Attack Settings")]
    public float attackCooldown = 1f; // 공격 간격
    public int damage = 10; // 공격력

    [Header("Movement Settings")]
    public float moveSpeed = 1f; // 이동 속도

    private Transform nearestTarget; // 가장 가까운 타겟
    private bool isAttacking = false; // 공격 중인지 확인
    private float attackTimer = 0f; // 공격 쿨타임 타이머

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // 가장 가까운 타겟 찾기
        nearestTarget = FindNearestTarget();

        if (nearestTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, nearestTarget.position);

            if (distanceToTarget <= attackRange)
            {
                // 공격 범위 내에 있으면 공격
                if (!isAttacking && attackTimer <= 0f)
                {
                    Attack(nearestTarget);
                }
            }
            else
            {
                // 공격 범위를 벗어나면 이동
                MoveTowards(nearestTarget.position);
            }
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
    }

    private void Attack(Transform target)
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        // 타겟에 데미지 주기 (예: Health 컴포넌트)
        

        // 공격 애니메이션 트리거
        // (예시) anim.SetTrigger("Attack");

        // 공격 종료 후 상태 초기화
        //isAttacking = false;
    }
}
