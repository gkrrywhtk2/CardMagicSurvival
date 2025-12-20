using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;

public class Card6_ArcaneOverDrive : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        RankType rank = card.currentPlayerCard.currentRarity;

        // 이펙트 연출
        GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(true);

        float duration = GetDurationByRank(rank);

        // ✅ 추가 치명타 확률 (0~100 단위로 쓰는 구조)
        float bonusCritChance = 100f;

        // ✅ PlayerCritical 참조
        var crit = GameManager.instance.player.playerStatus.playerCritical;
        if (crit == null)
            return;

        // ✅ 중첩 + 개별 만료는 PlayerCritical이 처리
        crit.AddBonusCritChanceTimed(bonusCritChance, duration);

        // ✅ 이펙트 끄는 타이밍도 duration 후 꺼야 하면 "별도 코루틴"만 이펙트용으로 둔다
        StartCoroutine(DisableEffectAfter(duration));
    }

    private System.Collections.IEnumerator DisableEffectAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        GameManager.instance.player.playerEffect.card6_Effect1.SetActive(false);
    }

    private float GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 1f;
            case RankType.Rare:      return 2f;
            case RankType.Epic:      return 3f;
            case RankType.Legendary: return 4f;
            default:                return 1f;
        }
    }
}
