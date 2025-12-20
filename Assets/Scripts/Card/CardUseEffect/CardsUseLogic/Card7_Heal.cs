using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;

public class Card7_Heal : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        Player_Status status = GameManager.instance.player.playerStatus;
        if (status == null || status.playerHP == null)
            return;

        RankType rank = card.currentPlayerCard.currentRarity;

        // 이펙트 연출 (원래 card6 이펙트 쓰고 있었는데, 네 프로젝트 사정상 유지)
        GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);

        // ✅ 최대 체력 기준 퍼센트 힐
        float healPercent = GetHealPerByRank(rank);   // 10/20/30/40
        float healValue = status.playerHP.maxHealth * (healPercent / 100f);

        // ✅ 체력 회복 (클램프는 playerHP.Heal 내부에서 처리)
        status.playerHP.Heal(healValue);
    }

    private float GetHealPerByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 10f;
            case RankType.Rare:      return 20f;
            case RankType.Epic:      return 30f;
            case RankType.Legendary: return 40f;
            default:                 return 10f;
        }
    }
}
