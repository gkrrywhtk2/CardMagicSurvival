using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
   public float damage;
   public int per;//관통 횟수
   public float bulletSpeed;
   public int effectId;//effectId는 effectpoolmanager에서 해당 불릿의 이펙트가 몇 번째 오브젝트인지를 의미함(폭발 이펙트)
   public enum bulletType{bullet};
   public bulletType type;
   public bool isCritical;

 
    

   Rigidbody2D rigid;
   private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
       
    }

     public void Init(float damage, int per, float bulletspeed,Vector3 dir, int effectId,
      bulletType type, bool isCritical)
    {
        this.damage = damage;
        this.per = per;
        this.bulletSpeed = bulletspeed;
        this.effectId = effectId;
        this.type = type;
        this.isCritical = isCritical;

         if (per > -1)
        {
            //Bullet의 관통 횟수가 남아있다면 Bullet을 dir * speed 의 속도로 발사.
            rigid.linearVelocity = dir * bulletspeed;
        }
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
{
    if (!collision.CompareTag("Monster")) return; // 한 번만 검사
    Monster enemy = collision.GetComponent<Monster>();
    if (enemy == null) return; // 예외 처리
        if (per == -1) return;

        enemy.DamageCalculator(damage, isCritical);
        per--;

        if (per <= 0) // 안전한 조건 검사
        {
            GameObject effect = GameManager.instance.effectPoolManager.Get(effectId);
            effect.transform.position = transform.position;
            rigid.linearVelocity = Vector3.zero;
            this.gameObject.SetActive(false);
        }
    enemy.CallHitStop(); // 몬스터 충돌 연산
}


   

     private void OnTriggerExit2D(Collider2D collision)
    {

        if(type != bulletType.bullet)
            return;

        if (collision.CompareTag("Wall"))
        {

            gameObject.SetActive(false);
        }


    }
    
}
