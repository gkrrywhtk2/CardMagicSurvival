using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card6_ArcaneOverDrive : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        // ✅ 등급 대신 레벨 사용
        int level = card.currentPlayerCard.LEVEL;

        // 이펙트 연출
        GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(true);

        // ✅ 레벨 기반 지속시간
        float duration = GetDurationByLevel(level);

        // ✅ 추가 치명타 확률 (0~100 단위로 쓰는 구조)
        float bonusCritChance = 100f;

        // ✅ PlayerCritical 참조
        var crit = GameManager.instance.player.playerStatus.playerCritical;
        if (crit == null)
            return;

        // ✅ 중첩 + 개별 만료는 PlayerCritical이 처리
        crit.AddBonusCritChanceTimed(bonusCritChance, duration);

        // ✅ 이펙트 끄기
        StartCoroutine(DisableEffectAfter(duration));
    }

    private IEnumerator DisableEffectAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(false);
        // 필요하면 Effect0도 끄기
        // GameManager.instance.player.playerEffect.card6_Effect0.SetActive(false);
    }

    /// <summary>
    /// ✅ 레벨 기반 지속시간 공식
    /// - Lv1  : 1초
    /// - Lv25 : 4초   (기존 Legendary 최대치에 맞춤)
    /// 선형 증가: 1 + (L-1) * (3/24) = 1 + (L-1)*0.125
    /// </summary>
    private float GetDurationByLevel(int level)
    {
        level = Mathf.Clamp(level, 1, 99);
        return 1f + (level - 1) * 0.125f; // Lv25 -> 4초
    }
}
