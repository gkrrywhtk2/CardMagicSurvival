using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card3_ManaFlow : MonoBehaviour, ICardUse
{
    private Coroutine manaRecoveryCoroutine; // 현재 실행 중인 코루틴을 저장할 변수

    public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();

        // 이펙트 연출
        GameManager.instance.player.playerEffect.PlayManaUp();

        // 지속시간 가져오기
        float duration = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
            .GetDuration(card.magicCard.STACK);

        // 마나 회복량 가져오기
        float recoveryAmount = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
            .GetManaRecovery(card.magicCard.STACK);

        // 기존 효과가 진행 중이라면 중지
        if (manaRecoveryCoroutine != null)
        {
            StopCoroutine(manaRecoveryCoroutine);
        }

        // 새 코루틴 시작 후 변수에 저장
        manaRecoveryCoroutine = StartCoroutine(TemporaryManaRecovery(recoveryAmount, duration));

    }

    public IEnumerator TemporaryManaRecovery(float percent, float duration)
    {
        Player_Status player = GameManager.instance.player.playerStatus;

        // 기존 효과 제거 (이미 적용된 효과가 있다면 리셋)
        player.RemoveManaRecoveryEffect(percent);
        player.AddManaRecoveryEffect(percent);

        //IconUI 생성
        GameManager.instance.iconManager.AddOrUpdateIcon(3, duration);

        yield return new WaitForSeconds(duration);

        // 지속시간 종료 후 효과 제거
        player.RemoveManaRecoveryEffect(percent);
        manaRecoveryCoroutine = null; // 코루틴 종료 시 변수 초기화
    }
}
