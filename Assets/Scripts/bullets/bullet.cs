using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
   public float damage;
   public int per;//관통 횟수
   public float bulletSpeed;
   public int effectId;//effectId는 effectpoolmanager에서 해당 불릿의 이펙트가 몇 번째 오브젝트인지를 의미함(폭발 이펙트)
   public enum bulletType{bullet, placement};
   public bulletType type;
   public bool isCritical;
   //placement only 
   public float duration;//placement타입 bullet의 지속시간
    private HashSet<Monster> affectedEnemies = new HashSet<Monster>();
     private HashSet<Boss> affectedEnemies_Boss = new HashSet<Boss>();

   Rigidbody2D rigid;
   private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
       
    }

     public void Init(float damage, int per, float bulletspeed,Vector3 dir, int effectId, bulletType type, bool isCritical)
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

        if(type == bulletType.placement){
            duration = 5f;//5초 임시
            StartCoroutine(DurationTime());
        }
    
    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Monster")){
                if(type == bulletType.bullet){
            if (!collision.CompareTag("Monster") || per == -1)
            return;
        collision.GetComponent<Monster>().DamageCalculator(damage,isCritical);
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

               // 몬스터와 충돌하면 도트 데미지 시작
        Monster enemy = collision.GetComponent<Monster>();

        if (enemy != null && !affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            StartCoroutine(ApplyDotDamage(enemy));
        }

        }
        }
    if(collision.CompareTag("Boss")){
                if(type == bulletType.bullet){
            if (!collision.CompareTag("Monster") || per == -1)
            return;
        collision.GetComponent<Boss>().DamageCalculator(damage,isCritical);
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

               // 몬스터와 충돌하면 도트 데미지 시작
        Boss boss = collision.GetComponent<Boss>();

        if (boss != null && !affectedEnemies_Boss.Contains(boss))
        {
            affectedEnemies_Boss.Add(boss);
            StartCoroutine(ApplyDotDamage_Boss(boss));
        }

        }
    }
        
       
    }
    
     private IEnumerator ApplyDotDamage(Monster enemy)
    {
        while (affectedEnemies.Contains(enemy))
        {
            // 몬스터에게 도트 데미지 적용
            enemy.DamageCalculator(damage, isCritical);
            
            enemy.speed = 0.3f;
            yield return new WaitForSeconds(1f); // 1초 간격으로 데미지
        }
    }
     private IEnumerator ApplyDotDamage_Boss(Boss enemy)
    {
        while (affectedEnemies_Boss.Contains(enemy))
        {
            // 몬스터에게 도트 데미지 적용
            enemy.DamageCalculator(damage, isCritical);
            
            enemy.speed = 0.3f;
            yield return new WaitForSeconds(1f); // 1초 간격으로 데미지
        }
    }

     private void OnTriggerExit2D(Collider2D collision)
    {

              // 독 지대를 벗어나면 도트 데미지 중지
            if(type == bulletType.placement){
            Monster enemy = collision.GetComponent<Monster>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Remove(enemy);
        }
        }
        

        if(type != bulletType.bullet)
            return;

        if (collision.CompareTag("Wall"))
        {

            gameObject.SetActive(false);
        }


    }

    IEnumerator DurationTime(){
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
    
}
