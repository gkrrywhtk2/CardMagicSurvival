using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;




public class Card5_FireBall : MonoBehaviour, ICardUse
{
   public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        RankType rank = card.currentPlayerCard.currentRarity;
        int fireBallNum = 7;
       
        Bullet_2 fireBall = GameManager.instance.objectPooling.Get(fireBallNum).GetComponent<Bullet_2>();
       // Vector2 cardVec = eventData.pointerDrag.GetComponent<Vector2>();
        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector2(eventData.position.x, eventData.position.y));
        Vector2 playerVec = GameManager.instance.player.playerCenterPivot.position;

        //생성시 플레이어의 위치로 생성 
        fireBall.GetComponent<Transform>().position = playerVec;
        
        Vector2 direction = (targetPosition - playerVec).normalized;
        float bulletSpeed = 7;
        int per = -1;//무한
        //레벨에 따른 조정(데미지, 지속시간)   
        float damage = GetDamageByRank(rank);
        fireBall.ScaleSetting(GetScaleByRank(rank));


        //  float damage = GameManager.instance.deckManager.cardDatas[card.magicCard.ID].GetDamage(card.magicCard.STACK);
        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(damage,out bool isCritical);
        fireBall.Init(direction,bulletSpeed,per,finalDamage,isCritical);
    }

      private int GetDamageByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return 5;
            case RankType.Rare: return 6;
            case RankType.Epic: return 7;
            case RankType.Legendary: return 8;
            default: return 5;
        }
    }
      private Vector3 GetScaleByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return new Vector3(1.5f,1.5f,1.5f);
            case RankType.Rare: return new Vector3(2f,2f,1);
            case RankType.Epic: return new Vector3(2.2f,2.2f,1);
            case RankType.Legendary: return new Vector3(2.5f,2.5f,1);
            default: return new Vector3(1,1,1);
        }
    }
}
