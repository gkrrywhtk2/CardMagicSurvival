using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using FunkyCode.Utilities;



public class Card5_FireBall : MonoBehaviour, ICardUse
{
   public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        int fireBallNum = 7;
       
        Bullet_2 fireBall = GameManager.instance.poolManager.Get(fireBallNum).GetComponent<Bullet_2>();
       // Vector2 cardVec = eventData.pointerDrag.GetComponent<Vector2>();
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector2(eventData.position.x, eventData.position.y));
        Vector2 playerVec = GameManager.instance.player.playerCenterPivot.position;

        //생성시 플레이어의 위치로 생성 
        fireBall.GetComponent<Transform>().position = playerVec;
        
        Vector2 direction = (targetPosition - playerVec).normalized;
        float bulletSpeed = 7;
        int per = -1;//무한
        //레벨에 따른 조정(데미지, 지속시간)   
        float damage = GameManager.instance.deckManager.cardDatas[card.magicCard.ID].GetDamage(card.magicCard.STACK);
        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(damage,out bool isCritical);
        fireBall.Init(direction,bulletSpeed,per,finalDamage,isCritical);
    }
}
