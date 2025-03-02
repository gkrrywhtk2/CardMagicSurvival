using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Placement : MonoBehaviour
{
     private float duration;//placement타입 bullet의 지속시간
     private HashSet<Monster> affectedEnemies = new HashSet<Monster>();
     public enum elementType{normal, posion};
     private elementType element;
     private float damage;
     private bool critical;

     //
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

     public void Init(float finalDamage, bool isCritical, float durationTime,  elementType elementType){
        damage = finalDamage;
        critical = isCritical;
        duration = durationTime;
        element = elementType;
        StartCoroutine(DurationTime());
     }


       private void OnTriggerEnter2D(Collider2D collision)
    {
    if (!collision.CompareTag("Monster")) return; // 한 번만 검사

    Monster enemy = collision.GetComponent<Monster>();
    if (enemy == null) return; // 예외 처리

        if (!affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Add(enemy);
            StartCoroutine(ApplyDotDamage(enemy));
        }
        enemy.CallHitStop(); // 몬스터 충돌 연산
    }

     private void OnTriggerExit2D(Collider2D collision)
    {

        // 독 지대를 벗어나면 도트 데미지 중지
        Monster enemy = collision.GetComponent<Monster>();
        if (enemy != null && affectedEnemies.Contains(enemy))
        {
            affectedEnemies.Remove(enemy);
        }
    }

     private IEnumerator ApplyDotDamage(Monster enemy)
    {
        while (affectedEnemies.Contains(enemy))
        {
            // 몬스터에게 도트 데미지 적용
            enemy.DamageCalculator(damage, critical);
            enemy.speed = 0.3f;


            //몬스터에게 맹독 데미지 적용
            enemy.toxicityPlus();//중독 수치 추가++
            enemy.ApplyPoison();//중독 데미지 실행
            yield return new WaitForSeconds(1f); // 1초 간격으로 데미지
        }
    }

    IEnumerator DurationTime(){
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}

