using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class bullet : MonoBehaviour
{
   public float damage;
   public int per;//관통 횟수
   public float bulletSpeed;
   Rigidbody2D rigid;
    public GameObject bulletEffect;
    public GameObject hitEffect;
   private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
       
    }

     public void Init(float damage, int per, float bulletspeed,Vector3 dir)
    {
        hitEffect.gameObject.SetActive(false);
        bulletEffect.gameObject.SetActive(true);
        this.damage = damage;
        this.per = per;
        this.bulletSpeed = bulletspeed;
         if (per > -1)
        {
            //Bullet의 관통 횟수가 남아있다면 Bullet을 dir * speed 의 속도로 발사.
            rigid.linearVelocity = dir * bulletspeed;
        }
    
    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Monster") || per == -1)
            return;
        collision.GetComponent<Monster>().DamageCalculator(damage);
        per--;
        if (per == -1)
        {
           bulletEffect.gameObject.SetActive(false);
           hitEffect.gameObject.SetActive(true);
             rigid.linearVelocity = Vector3.zero;

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
