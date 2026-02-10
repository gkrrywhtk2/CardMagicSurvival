using UnityEngine;
using Game.RankSystem;

[System.Serializable]
public class PlayerCard
{
    // ✅ 개별 카드 인스턴스 정보 (덱, 핸드에서 사용)
    public int ID;               // 카드 고유 ID
    public int LEVEL;   // 현재 카드 레벨

    public PlayerCard(int cardId, int level)
    {
        ID = cardId;
        LEVEL = level;
    }
}