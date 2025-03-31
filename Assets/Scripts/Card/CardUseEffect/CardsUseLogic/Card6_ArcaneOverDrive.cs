using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card6_ArcaneOverDrive : MonoBehaviour, ICardUse
{
     private Coroutine criticalCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();

        // 이펙트 연출
        GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);
        // 이펙트 연출
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(true);

        // 지속시간 가져오기
        float duration = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
            .GetDuration(card.magicCard.STACK);

        //추가 치명타 확률
        float percent = 100;

        // 기존 효과가 진행 중이라면 중지
        if (criticalCoroutine != null)
        {
            StopCoroutine(criticalCoroutine);
        }

        // 새 코루틴 시작 후 변수에 저장
        criticalCoroutine = StartCoroutine(VisionOverDrive(duration, percent));


    }

    public IEnumerator VisionOverDrive(float duration, float per)
    {
        Player_Status player = GameManager.instance.player.playerStatus;

        // 기존 효과 제거 (이미 적용된 효과가 있다면 리셋)
        player.RemoveCriticalEffect(per);
        player.AddCriticalEffect(per);
        player.InitALLStat();//스탯 적용

        //IconUI 생성
        GameManager.instance.iconManager.AddOrUpdateIcon(6, duration);

        yield return new WaitForSeconds(duration);

        // 지속시간 종료 후 효과 제거
        player.RemoveCriticalEffect(per);
         player.InitALLStat();//스탯 적용
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(false);
        criticalCoroutine = null; // 코루틴 종료 시 변수 초기화
    }
}
