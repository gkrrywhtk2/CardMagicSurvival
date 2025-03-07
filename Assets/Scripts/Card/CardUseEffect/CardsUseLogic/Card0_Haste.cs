using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card0_Haste : MonoBehaviour, ICardUse
{
    private Coroutine hasteCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();

        // 이펙트 연출
        GameManager.instance.player.playerEffect.HasteEffect();

        // 지속시간 가져오기
        float duration = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
            .GetDuration(card.magicCard.STACK);

        // 추가 이동속도 값 가져오기
        float speedUpValue = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
            .GetSpeedUp(card.magicCard.STACK);

        // 기존 효과가 진행 중이라면 중지
        if (hasteCoroutine != null)
        {
            StopCoroutine(hasteCoroutine);
        }

        // 새 코루틴 시작 후 변수에 저장
        hasteCoroutine = StartCoroutine(TemporarySpeedUp(speedUpValue, duration));


    }

    public IEnumerator TemporarySpeedUp(float value, float duration)
    {
        Player_Status player = GameManager.instance.player.playerStatus;

        // 기존 효과 제거 (이미 적용된 효과가 있다면 리셋)
        player.RemoveSpeedUpEffect(value);
        player.AddSpeedUpEffect(value);

        //IconUI 생성
        GameManager.instance.iconManager.AddOrUpdateIcon(0, duration);

        yield return new WaitForSeconds(duration);

        // 지속시간 종료 후 효과 제거
        player.RemoveSpeedUpEffect(value);
        hasteCoroutine = null; // 코루틴 종료 시 변수 초기화
    }
}
