using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Game.RankSystem;


public class Card4_FlameBurst : MonoBehaviour, ICardUse
{
      public Coroutine flameBurstCorutine;

      public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        RankType rank = card.currentPlayerCard.currentRarity;
        // 기존 Coroutine이 실행 중이면 중단
        if (flameBurstCorutine != null)
        {
            StopCoroutine(flameBurstCorutine);
        }
    

        // 새로운 Coroutine 시작
        flameBurstCorutine = StartCoroutine(FlameBurstRoutine(rank));

    }

       private IEnumerator FlameBurstRoutine(RankType rank){

        int repeatCount = GetRepeatCountByRank(rank);
        float damage = GetDamageByRank(rank);
        Vector3 scale = GetScaleByRank(rank);
        int flameburstObjectNum = 7; // 오브젝트 풀에서 가져올 ID

        //repeatCount만큼 반복 -> repeatCount만큼 화염 생성
        for(int i = 0; i < repeatCount; i++){

            //데미지 세팅
            float finalDamage = GameManager.instance.player.playerStatus
                .DamageReturn(damage,out bool isCritical);

            // FlameBurst 효과 생성
            GameObject flame = GameManager.instance.effectPoolManager.Get(flameburstObjectNum);
            flame.GetComponent<Melee>().Init(finalDamage,isCritical);
            flame.GetComponent<Melee>().ScaleSetting(scale);
            //플레이어의 바로 앞 스킬이 연출될 좌표
            Vector2 skillPosition; 

            float angle;//각도
            //player의 dir_front에서 좌표 가져옴
            skillPosition = GameManager.instance.player.dirFront.skillPosition;
            angle = GameManager.instance.player.dirFront.angle;
            flame.transform.position = skillPosition;
            flame.transform.rotation = Quaternion.Euler(0, 0, angle);

            //0.2초 마다 반복
            yield return new WaitForSeconds(0.25f);
            }

            //코루틴 null 갱신
            flameBurstCorutine = null;
      }

    private int GetRepeatCountByRank(RankType rank)
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
