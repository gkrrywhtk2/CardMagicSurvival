using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Game.RankSystem;

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

        var rank = card.currentPlayerCard.currentRarity;

        // 이펙트 연출
        GameManager.instance.player.playerEffect.HasteEffect();

        // 랭크 기반 지속시간
        float duration = GetDurationByRank(rank);

        // 랭크별 속도 증가량도 원하면 여기서 분기 가능
        float speedUpValue = 1f;

        // ✅ 신규 로직: PlayerMoveSpeed가 ID 기반/시간제/중첩/만료를 모두 처리
        var moveSpeed = GameManager.instance.player.GetComponent<PlayerMoveSpeed>();
        if (moveSpeed == null) return;

        moveSpeed.AddBonusSpeedTimed(speedUpValue, duration);

        // (선택) 이펙트를 duration 후 끄고 싶다면
        if (effectCoroutine != null) StopCoroutine(effectCoroutine);
        effectCoroutine = StartCoroutine(DisableEffectAfter(duration));
    }

    private IEnumerator DisableEffectAfter(float duration)
    {
        yield return new WaitForSeconds(duration);

        // 너 이펙트 시스템에 OFF 함수가 있으면 그걸 호출
        // 예: GameManager.instance.player.playerEffect.HasteEffectOff();
        // 없으면 SetActive(false) 등으로 처리
    }

    private float GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 1f;
            case RankType.Rare:      return 2f;
            case RankType.Epic:      return 3f;
            case RankType.Legendary: return 4f;
            default:                 return 1f;
        }
    }
}
