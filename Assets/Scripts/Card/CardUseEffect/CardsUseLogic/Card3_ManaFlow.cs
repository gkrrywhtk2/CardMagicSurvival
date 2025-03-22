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

   public IEnumerator TemporaryManaRecovery(float flatValue, float duration)
{
    Player_Status player = GameManager.instance.player.playerStatus;

    // 기존 동일한 수치 제거 (중복 방지용)
    player.RemoveManaRecoveryFlat(flatValue);
    player.AddManaRecoveryFlat(flatValue);

    // UI 아이콘 표시 (예: 3번 슬롯에 duration 동안 표시)
    GameManager.instance.iconManager.AddOrUpdateIcon(3, duration);

    yield return new WaitForSeconds(duration);

    // 지속시간이 끝나면 해당 효과 제거
    player.RemoveManaRecoveryFlat(flatValue);
    manaRecoveryCoroutine = null;
}

}
