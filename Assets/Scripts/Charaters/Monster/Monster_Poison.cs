using System.Collections;
using UnityEngine;

public class Monster_Poison : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private GameObject toxicityEffect; // 프리팹에서 할당

    [Header("Tick")]
    [SerializeField] private float tickSeconds = 1f;

    private Monster owner;
    private Coroutine co;

    private float remain;
    private float dps;

    private void Awake()
    {
        owner = GetComponent<Monster>();
        if (toxicityEffect != null) toxicityEffect.SetActive(false);
    }

    private void OnDisable()
    {
        Clear();
    }

    /// <summary>
    /// ✅ 중독 적용/갱신
    /// 초당 100% 데미지 = dps로 전달한 값을 매 tick마다 1번 때림
    /// </summary>
    public void Apply(float poisonDps, float duration)
    {
        // 정책: 더 강한 독 / 더 긴 지속 유지
        dps = Mathf.Max(dps, poisonDps);
        remain = Mathf.Max(remain, duration);

        if (toxicityEffect != null) toxicityEffect.SetActive(true);

        if (co == null)
            co = StartCoroutine(Run());
    }

    public void Clear()
    {
        if (co != null)
        {
            StopCoroutine(co);
            co = null;
        }

        remain = 0f;
        dps = 0f;

        if (toxicityEffect != null) toxicityEffect.SetActive(false);
    }

    private IEnumerator Run()
    {
        while (remain > 0f)
        {
            if (owner == null || !owner.IsLive)
                break;

            owner.TakeDamage(dps, false, DamageType.Poison);

            yield return new WaitForSeconds(tickSeconds);
            remain -= tickSeconds;
        }

        co = null;
        Clear();
    }
}
