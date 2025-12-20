using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;

public class Card3_ManaFlow : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        RankType rank = card.currentPlayerCard.currentRarity;

        // 이펙트
        GameManager.instance.player.playerEffect.PlayManaUp();

        // 지속시간 / 추가 회복량
        float duration = GetDurationByRank(rank);
        float bonusFlatRecovery = GetBonusFlatRecoveryByRank(rank);

        // PlayerMana 가져오기
        PlayerMana mana = GameManager.instance.player.playerStatus.playerMana;
        if (mana == null)
            return;

        // ✅ 핵심: "추가 회복량"을 시간제로 추가 (중첩 가능, 각자 만료)
        mana.AddBonusManaRecoveryFlatTimed(bonusFlatRecovery, duration);
    }

    private float GetDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return 1f;
            case RankType.Rare: return 1.5f;
            case RankType.Epic: return 2f;
            case RankType.Legendary: return 2.5f;
            default: return 1f;
        }
    }

    // 등급별 추가 회복량(원하면 조절)
    private float GetBonusFlatRecoveryByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon: return 1f;
            case RankType.Rare: return 1f;
            case RankType.Epic: return 1f;
            case RankType.Legendary: return 1f;
            default: return 1f;
        }
    }
}
