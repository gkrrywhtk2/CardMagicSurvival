using UnityEngine;

public class Melee : MonoBehaviour
{
    public float damage;
     public void Init(float damage){
        this.damage = damage;
     }


      private void OnTriggerEnter2D(Collider2D collision){
        //몬스터와 충돌시 데미지 처리
        if(collision.CompareTag("Monster")){
         Monster enemy = collision.GetComponent<Monster>();
         enemy.DamageCalculator(damage);
        }
      }

      public void EndAnimation(){
        //애니메이션이 종료되고 오브젝트 비활성화
        gameObject.SetActive(false);
      }
}
