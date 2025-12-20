using UnityEngine;

public class Scaner : MonoBehaviour
{
    [Header("Scan")]
    public float scanrange_Attack;
    public LayerMask targetlayer;
    public Transform nearestTarget;

    [Header("Cache")]
    [SerializeField] private PlayerHP playerHp;

    [Header("Buffer")]
    [SerializeField] private int bufferSize = 64;
    private Collider2D[] results;
    private ContactFilter2D filter;

    private void Awake()
    {
        results = new Collider2D[bufferSize];

        // ✅ LayerMask 필터 세팅
        filter = new ContactFilter2D();
        filter.SetLayerMask(targetlayer);
        filter.useLayerMask = true;
        filter.useTriggers = true; // 필요에 따라 false로

        // ✅ PlayerHP 캐싱 (인스펙터 우선)
        if (playerHp == null)
        {
            var status = GameManager.instance?.player?.playerStatus;
            if (status != null) playerHp = status.playerHP;
        }
    }

    private void FixedUpdate()
    {
        if (playerHp != null && !playerHp.isLive) return;

        int count = Physics2D.OverlapCircle((Vector2)transform.position, scanrange_Attack, filter, results);
        nearestTarget = GetNearest(count);
    }

    private Transform GetNearest(int count)
    {
        Transform best = null;
        float bestDist = float.MaxValue;
        Vector2 myPos = transform.position;

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (col == null) continue;

            float d = ((Vector2)col.transform.position - myPos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = col.transform;
            }

            results[i] = null; // ✅ 다음 프레임 잔상 방지(선택)
        }

        return best;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, scanrange_Attack);
    }
#endif
}
