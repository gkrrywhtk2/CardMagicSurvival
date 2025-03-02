using UnityEngine;
using UnityEngine.EventSystems;
public class Card2_VenomousCurse : MonoBehaviour, ICardUse
{
       public void Use(PointerEventData eventData){
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();

        int splashEffect = 4;
        int poisonEffect = 5;
        GameObject poisonSplash = GameManager.instance.effectPoolManager.Get(splashEffect); 
        GameObject poison_temp = GameManager.instance.effectPoolManager.Get(poisonEffect); 
        Bullet_Placement posion = poison_temp.GetComponent<Bullet_Placement>();

        // 드랍 포인트를 목표로 초기화
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
        targetPosition.z = 0; // 2D 환경이라면 Z축을 고정
        poisonSplash.transform.position = targetPosition;
        posion.transform.position = targetPosition;

        //레벨에 따른 조정(데미지, 지속시간)   
        float damage = GameManager.instance.deckManager.cardDatas[card.magicCard.ID].GetDamage(card.magicCard.STACK);
        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(damage,out bool isCritical);
        float duration =  GameManager.instance.deckManager.cardDatas[card.magicCard.ID].GetDuration(card.magicCard.STACK);
      // global::bullet.bulletType type = global::bullet.bulletType.placement;
        posion.Init(finalDamage,isCritical,duration, Bullet_Placement.elementType.posion);
    }

   
   
}
