using System.Collections.Generic;
using UnityEngine;

public class AccountCardManager : MonoBehaviour
{
    public MockServer mockServer;

    public List<PlayerCard> accountCardPool = new(); // 전체 카드
    public List<int> deckSlots = new();              // 서버에 저장된 선택된 카드 ID 5개

    [System.Serializable]
    private class AccountCardData
    {
        public PlayerCard[] cards;
        public int[] deckSlots;
    }

    private void Awake()
    {
       // DontDestroyOnLoad(gameObject);
    }

    public void LoadFromServer()
    {
        string json = mockServer.GetPlayerCardJson();
        AccountCardData data = JsonUtility.FromJson<AccountCardData>(json);

        accountCardPool = new List<PlayerCard>(data.cards);
        deckSlots = new List<int>(data.deckSlots);

        Debug.Log($"[AccountCardManager] 카드 {accountCardPool.Count}개, 덱 {deckSlots.Count}개 로드 완료");
    }

    public void SaveToServer()
    {
        AccountCardData data = new AccountCardData
        {
            cards = accountCardPool.ToArray(),
            deckSlots = deckSlots.ToArray()
        };

        string json = JsonUtility.ToJson(data, true);
        mockServer.SavePlayerCardJson(json);
    }

    // ✅ [추가] 인게임 매니저가 덱 슬롯 ID를 가져갈 수 있도록 하는 함수
    public List<int> GetDeckSlotIds()
    {
        // 리스트의 복사본을 반환하여 외부 수정 방지
        return new List<int>(deckSlots);
    }
    
     // ✅ 현재 불러온 카드풀과 덱 슬롯 상태 출력 함수
    public void PrintDebugInfo()
    {
        Debug.Log("<color=#00FFFF>==== [AccountCardManager] 현재 계정 카드 풀 ====</color>");
        foreach (var card in accountCardPool)
        {
            Debug.Log($"[CardPool] 카드ID: {card.cardId}, 등급: {card.currentRarity}, 보유량: {card.quantity}, 잠금여부: {card.islocked}");
        }

        Debug.Log("<color=#FFA500>==== [AccountCardManager] 현재 덱 슬롯 ====</color>");
        for (int i = 0; i < deckSlots.Count; i++)
        {
            int cardId = deckSlots[i];
            var card = accountCardPool.Find(c => c.cardId == cardId);

            if (card != null)
                Debug.Log($"[DeckSlot {i}] 카드ID: {cardId}, 등급: {card.currentRarity}, 보유량: {card.quantity}");
            else
                Debug.LogWarning($"[DeckSlot {i}] ⚠️ 카드ID {cardId}는 카드풀에 존재하지 않습니다!");
        }

        Debug.Log("<color=#00FF00>===========================================</color>");
    }
}
