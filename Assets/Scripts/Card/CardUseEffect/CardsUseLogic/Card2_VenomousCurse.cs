using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;
public class Card2_VenomousCurse : MonoBehaviour, ICardUse
{
       public void Use(PointerEventData eventData){
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        var rank = card.currentPlayerCard.currentRarity;

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


        float magicDamage = GetDamageByRank(rank);//기본 데미지
        float duration = GetDurationByRank(rank);

        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(magicDamage,out bool isCritical);//최종 데미지(아이템 수치 반영)
        posion.Init(finalDamage, isCritical, duration, Bullet_Placement.elementType.posion);
    }

    private float GetDamageByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:   return  1;
            case RankType.Rare:       return 1.5f;
            case RankType.Epic:       return 2;
            case RankType.Legendary:  return 2.5f;
            default:                 return 1;
        }
    }

    private float GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:   return  3;
            case RankType.Rare:       return 4;
            case RankType.Epic:       return 5;
            case RankType.Legendary:  return 6;
            default:                 return 1;
        }
    }
}
