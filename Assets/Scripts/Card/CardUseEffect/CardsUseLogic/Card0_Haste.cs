using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card0_Haste : MonoBehaviour, ICardUse
{
    // (선택) 이펙트 OFF용 코루틴만 별도로 관리
    private Coroutine effectCoroutine;

    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        var card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;
        
        // ✅ SO에서 데이터 가져오기
        var data = card.cardScritableData;

        // ✅ 등급(Rank) 대신 레벨(Level) 사용
        int level = card.currentPlayerCard.LEVEL;

        // 이펙트 연출
        GameManager.instance.player.playerEffect.HasteEffect();

        // ✅ 레벨 기반 지속시간 (원하면 나중에 공식화 가능)
        float duration = data.GetDuration(level);
    
        // ✅ 레벨 기반 추가 이동속도
        // 요구사항: LEVEL 25쯤에서 추가 이동속도 ≈ 4
        float speedUpValue = data.GetSpeedUp(level);

        // ✅ 신규 로직: PlayerMoveSpeed가 시간제/중첩/만료 처리
        var moveSpeed = GameManager.instance.player.GetComponent<PlayerMoveSpeed>();
        if (moveSpeed == null) return;

        moveSpeed.AddBonusSpeedTimed(speedUpValue, duration);

        // (선택) duration 후 이펙트 끄기
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(DisableEffectAfter(duration));
    }

    private IEnumerator DisableEffectAfter(float duration)
    {
        yield return new WaitForSeconds(duration);

        // OFF 함수가 있으면 호출
        // GameManager.instance.player.playerEffect.HasteEffectOff();
    }

    /// <summary>
    /// ✅ 레벨 공식
    /// - Lv1  : +1.0
    /// - Lv25 : +4.0
    /// 선형 증가: 1 + (L-1) * (3/24) = 1 + (L-1)*0.125
    /// </summary>
}
