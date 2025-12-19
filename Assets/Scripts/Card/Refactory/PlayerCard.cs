using UnityEngine;
using Game.RankSystem;

[System.Serializable]
public class PlayerCard
{
    // ✅ 개별 카드 인스턴스 정보 (덱, 핸드에서 사용)
    public int ID;               // 카드 고유 ID
    public RankType currentRarity;   // 현재 등급

    public PlayerCard(int cardId, RankType rarity = RankType.Uncommon)
    {
        ID = cardId;
        currentRarity = rarity;
    }
}