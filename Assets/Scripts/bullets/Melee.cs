using UnityEngine;

public class Melee : MonoBehaviour
{
    public float damage;
    public bool isCritical;
     public void Init(float damage, bool iscritical){
        this.damage = damage;
        isCritical = iscritical;
     }


      private void OnTriggerEnter2D(Collider2D collision){
        //몬스터와 충돌시 데미지 처리
        if(collision.CompareTag("Monster")){
         Monster enemy = collision.GetComponent<Monster>();
         enemy.DamageCalculator(damage,isCritical);
        }
      }

      public void EndAnimation(){
        //애니메이션이 종료되고 오브젝트 비활성화
        gameObject.SetActive(false);
      }
}
