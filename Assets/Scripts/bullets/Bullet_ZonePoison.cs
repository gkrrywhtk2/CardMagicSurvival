using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ZonePoison : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private float zoneDuration;     // 장판 지속시간
    [SerializeField] private float poisonDuration;   // 중독 지속시간
    [SerializeField] private float poisonDps;        // 초당 데미지(= 100%라면 그대로 전달)

    private Coroutine lifeCo;
    private readonly HashSet<Monster> applied = new HashSet<Monster>();

    public void Init(float dps, float zoneDurationTime, float poisonDurationTime)
    {
        poisonDps = dps;
        zoneDuration = zoneDurationTime;
        poisonDuration = poisonDurationTime;

        // 풀링 재사용 대비
        applied.Clear();
        if (lifeCo != null) StopCoroutine(lifeCo);
        lifeCo = StartCoroutine(Life());
    }

    private void OnEnable()
    {
        applied.Clear();
    }

    private void OnDisable()
    {
        if (lifeCo != null)
        {
            StopCoroutine(lifeCo);
            lifeCo = null;
        }
        applied.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster")) return;

        Monster enemy = collision.GetComponent<Monster>();
        if (enemy == null) return;

        // 같은 장판에서 같은 몬스터에게 중복 적용 호출 방지
        if (!applied.Add(enemy)) return;

        Monster_Poison poison = enemy.GetComponent<Monster_Poison>();
        if (poison != null)
        {
            poison.Apply(poisonDps, poisonDuration);
        }
    }

    private IEnumerator Life()
    {
        yield return new WaitForSeconds(zoneDuration);
        lifeCo = null;
        gameObject.SetActive(false);
    }
}
