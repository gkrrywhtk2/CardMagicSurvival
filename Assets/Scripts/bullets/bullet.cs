using UnityEngine;

public class bullet : MonoBehaviour
{
   public float damage;
   public int per;//관통 횟수
   public float bulletSpeed;
   public int effectId;//effectId는 effectpoolmanager에서 해당 불릿의 이펙트가 몇 번째 오브젝트인지를 의미함
   public enum bulletType{bullet, placement};
   public bulletType type;

   Rigidbody2D rigid;
   private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
       
    }

     public void Init(float damage, int per, float bulletspeed,Vector3 dir, int effectId, bulletType type)
    {
        this.damage = damage;
        this.per = per;
        this.bulletSpeed = bulletspeed;
        this.effectId = effectId;
        this.type = type;
         if (per > -1)
        {
            //Bullet의 관통 횟수가 남아있다면 Bullet을 dir * speed 의 속도로 발사.
            rigid.linearVelocity = dir * bulletspeed;
        }
    
    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if(type == bulletType.bullet){
            if (!collision.CompareTag("Monster") || per == -1)
            return;
        collision.GetComponent<Monster>().DamageCalculator(damage);
        per--;
        if (per == -1)
        {
            GameObject effect = GameManager.instance.effectPoolManager.Get(effectId);
            effect.transform.position = transform.position;
            rigid.linearVelocity = Vector3.zero;
            this.gameObject.SetActive(false);

        }
        }
        else if(type == bulletType.placement){
            
        }
       
    }

     private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {

            gameObject.SetActive(false);
        }


    }
    
}
