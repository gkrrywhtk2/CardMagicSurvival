using RANK;
using UnityEngine;
using Game.RankSystem;

[System.Serializable]
public class AccountCard
{
    // ✅ 계정 레벨의 카드 보유 정보
    public int cardId;           // 카드 고유 ID
    public int quantity;         // 보유 수량
    public bool isUnlocked;      // 잠금 해제 여부 (islocked → isUnlocked로 의미 명확화)

    public AccountCard(int id)
    {
        cardId = id;
        quantity = 0;
        isUnlocked = false;
    }
}