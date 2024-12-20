using UnityEngine;

public class Scaner : MonoBehaviour
{
     [Header("Scan")]
    public float scanrange_Attack;//공격 범위
    public LayerMask targetlayer;//감지할 레이어
    public RaycastHit2D[] attackTargets;//감지된 타겟들
    public Transform nearestTarget;//가장 가까운 타겟 
     [Header("Connect")]
    Animator anim;

     private void FixedUpdate()
    {

        if (GameManager.instance.player.isLive != true)
            return;

        attackTargets = Physics2D.CircleCastAll(transform.position, scanrange_Attack, Vector2.zero, 0, targetlayer);
        nearestTarget = GetNearest_Attack();
    }

     Transform GetNearest_Attack()
    {
        Transform result = null;
        float diff = 100;

        foreach (RaycastHit2D target in attackTargets)
        {
            Vector3 mypos = transform.position;
            Vector3 targetpos = target.transform.position;
            float curDiff = Vector3.Distance(mypos, targetpos);

            if (curDiff < diff)
            {
                diff = curDiff;
                result = target.transform;
            }

        }
        return result;
    }
}
