using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ExplosionNormal : MonoBehaviour
{
    [Header("Explosion")]
    [SerializeField] private float damage;
    [SerializeField] private bool critical;

    private Coroutine lifeCo;
    private readonly HashSet<Monster> hit = new HashSet<Monster>();

    public void Init(float finalDamage, bool isCritical)
    {
        damage = finalDamage;
        critical = isCritical;
        // 풀링 재사용 대비
        hit.Clear();
        if (lifeCo != null) StopCoroutine(lifeCo);
    }

    private void OnEnable()
    {
        hit.Clear();
    }

    private void OnDisable()
    {
        if (lifeCo != null)
        {
            StopCoroutine(lifeCo);
            lifeCo = null;
        }
        hit.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster")) return;

        Monster enemy = collision.GetComponent<Monster>();
        if (enemy == null) return;

        // 같은 폭발에서 같은 몬스터 중복 타격 방지
        if (!hit.Add(enemy)) return;

        enemy.TakeDamage(damage, critical, DamageType.Normal);
    }

    public void EffectEnd()
    {
        gameObject.SetActive(false);
    }
}
