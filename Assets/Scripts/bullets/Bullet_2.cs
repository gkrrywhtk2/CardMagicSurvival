using System.Runtime.InteropServices;
using UnityEngine;

public class Bullet_2 : MonoBehaviour
{

    private Rigidbody2D rb;
    private int perCount;//관통 횟수
    private float angle;
    private float bulletDamage;
    private bool bulletIsCritical;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, int per, float damage, bool isCritical)
    {
        rb.linearVelocity = direction * speed;
        perCount = per;

        //회전 설정
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        //데미지 세팅
        bulletDamage = damage;
        bulletIsCritical = isCritical;
    
    }

     private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Monster")) return; // 한 번만 검사
        Monster enemy = collision.GetComponent<Monster>();

        //예외 처리
        if (enemy == null) return;

        //데미지 처리
        enemy.DamageCalculator(bulletDamage, bulletIsCritical);
        //몬스터 충돌 연출
        enemy.CallHitStop();


        //무한 관통 총알 예외 처리
        if (perCount == -1) return;
        perCount--;

        /**
        if (perCount <= 0) // 안전한 조건 검사
        {
            GameObject effect = GameManager.instance.effectPoolManager.Get(effectId);
            effect.transform.position = transform.position;
            rb.linearVelocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }
        **/
}



    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }

    
}
