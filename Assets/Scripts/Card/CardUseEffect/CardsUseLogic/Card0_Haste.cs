using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Game.RankSystem;

public class Card0_Haste : MonoBehaviour, ICardUse
{
    private Coroutine hasteCoroutine;

    private bool isApplied;
    private float appliedValue;

    public void Use(PointerEventData eventData)
    {
        var card = eventData.pointerDrag.GetComponent<MagicCard>();
        var rank = card.currentPlayerCard.currentRarity;

        // 이펙트 연출
        GameManager.instance.player.playerEffect.HasteEffect();

        // 랭크 기반 지속시간
        float duration = GetDurationByRank(rank);

        // (원하면 이것도 랭크 기반으로 바꾸면 됨)
        float speedUpValue = 1f;

        // 기존 코루틴/효과 정리
        if (hasteCoroutine != null)
        {
            StopCoroutine(hasteCoroutine);
            RemoveAppliedEffect();
            hasteCoroutine = null;
        }

        hasteCoroutine = StartCoroutine(TemporarySpeedUp(speedUpValue, duration));
    }

    private IEnumerator TemporarySpeedUp(float value, float duration)
    {
        var player = GameManager.instance.player.playerStatus;

        // 안전하게 기존 적용값 제거 후 다시 적용
        RemoveAppliedEffect();

        appliedValue = value;
        isApplied = true;
        player.AddSpeedUpEffect(value);

        yield return new WaitForSeconds(duration);

        RemoveAppliedEffect();
        hasteCoroutine = null;
    }

    private void RemoveAppliedEffect()
    {
        if (!isApplied) return;

        var player = GameManager.instance.player.playerStatus;
        player.RemoveSpeedUpEffect(appliedValue);

        isApplied = false;
        appliedValue = 0f;
    }

    private float GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:   return 1f;
            case RankType.Rare:       return 2f;
            case RankType.Epic:       return 3f;
            case RankType.Legendary:  return 4f;
            default:                 return 1f;
        }
    }
}
