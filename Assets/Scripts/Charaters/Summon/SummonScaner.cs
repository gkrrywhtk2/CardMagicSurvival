using System.Collections;
using UnityEngine;

public class SummonScaner : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRange = 5f; // 탐지 범위
    public float attackRange = 1f; // 공격 범위
    public LayerMask targetLayer; // 탐지할 타겟 레이어
    public Transform nearestTarget; // 가장 가까운 타겟
    public Transform attackTarget; // 가장 가까운 공격 타겟

     private void FixedUpdate(){
        nearestTarget = FindNearestTarget();
        attackTarget = FindNearestAttackTarget();

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

     private Transform FindNearestAttackTarget()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, attackRange, targetLayer);
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

/**
스캐너 범위 scene에서 확인하기 위한 함수
    private void OnDrawGizmosSelected()
{
    // 탐지 범위 색상 설정
    Gizmos.color = Color.green;

    // 탐지 범위 원 그리기
    Gizmos.DrawWireSphere(transform.position, detectionRange);

    // 공격 범위 시각화 (옵션)
    Gizmos.color = Color.red;
    Gizmos.DrawWireSphere(transform.position, attackRange);
}
**/
}
