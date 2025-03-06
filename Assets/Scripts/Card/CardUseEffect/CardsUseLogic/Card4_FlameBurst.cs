using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;


public class Card4_FlameBurst : MonoBehaviour, ICardUse
{
      public Coroutine flameBurstCorutine;
      private int magicID = 4;

      public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        // 기존 Coroutine이 실행 중이면 중단
        if (flameBurstCorutine != null)
        {
            StopCoroutine(flameBurstCorutine);
        }
    
        int STACK = card.magicCard.STACK;
        // 새로운 Coroutine 시작
        flameBurstCorutine = StartCoroutine(FlameBurstRoutine(STACK));

    }

       private IEnumerator FlameBurstRoutine(int stack){
        int repeatCount = GameManager.instance.deckManager.cardDatas[magicID].GetCount(stack);
        float damage = GameManager.instance.deckManager.cardDatas[magicID].GetDamage(stack);
        int flameburstObjectNum = 7; // 오브젝트 풀에서 가져올 ID

        //repeatCount만큼 반복 -> repeatCount만큼 화염 생성
        for(int i = 0; i < repeatCount; i++){

            //데미지 세팅
            float finalDamage = GameManager.instance.player.playerStatus
                .DamageReturn(damage,out bool isCritical);

            // FlameBurst 효과 생성
            GameObject flame = GameManager.instance.effectPoolManager.Get(flameburstObjectNum);
            flame.GetComponent<Melee>().Init(finalDamage,isCritical);

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

}
