using System.Collections;
using UnityEngine;

public class bullet : MonoBehaviour
{
    public float damage;
    public int per;
    public float bulletSpeed;
    public int effectId;
    public enum bulletType { bullet };
    public bulletType type;
    public bool isCritical;

    [Header("Lifetime")]
    [SerializeField] private float lifeTime = 10f;
    private Coroutine lifeRoutine;

    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, float bulletspeed, Vector3 dir, int effectId,
        bulletType type, bool isCritical)
    {
        this.damage = damage;
        this.per = per;
        this.bulletSpeed = bulletspeed;
        this.effectId = effectId;
        this.type = type;
        this.isCritical = isCritical;

        // 풀 재사용 대비: 속도/상태 초기화
        rigid.linearVelocity = dir * bulletspeed;

        // 10초 후 자동 비활성화 타이머 리셋
        StartLifeTimer();
        
    }

    private void StartLifeTimer()
    {
        if (lifeRoutine != null)
            StopCoroutine(lifeRoutine);

        lifeRoutine = StartCoroutine(LifeTimer());
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);

        if (gameObject.activeInHierarchy)
        {
            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
        }

        lifeRoutine = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster")) return;

        Monster enemy = collision.GetComponent<Monster>();
        if (enemy == null) return;

        // ✅ 항상 데미지는 준다 (무한 관통이어도 데미지는 들어가야 함)
        enemy.DamageCalculator(damage, isCritical);

        // ✅ 무한 관통이면 여기서 끝 (사라지지 않음, per도 안 줄임)
        if (per == -1)
        {
            enemy.CallHitStop();
            return;
        }

        // ✅ 유한 관통이면 per 감소
        per--;

        if (per <= 0)
        {
            GameObject effect = GameManager.instance.effectPoolManager.Get(effectId);
            effect.transform.position = transform.position;

            rigid.linearVelocity = Vector2.zero;
            gameObject.SetActive(false);
            return;
        }

        enemy.CallHitStop();
    }


    private void OnDisable()
    {
        // 풀로 돌아갈 때 타이머 정리(재사용 시 중복 방지)
        if (lifeRoutine != null)
        {
            StopCoroutine(lifeRoutine);
            lifeRoutine = null;
        }

        // 혹시 모를 잔재 방지(선택)
        if (rigid != null)
            rigid.linearVelocity = Vector2.zero;
    }
}
