using UnityEngine;

public class MockServer : MonoBehaviour
{
    private string mockJson = @"
    {
        ""accountCards"": [
            { ""cardId"": 0, ""quantity"": 3, ""isUnlocked"": true },
            { ""cardId"": 1, ""quantity"": 2, ""isUnlocked"": true },
            { ""cardId"": 2, ""quantity"": 5, ""isUnlocked"": true },
            { ""cardId"": 3, ""quantity"": 1, ""isUnlocked"": true },
            { ""cardId"": 4, ""quantity"": 4, ""isUnlocked"": true },
            { ""cardId"": 5, ""quantity"": 2, ""isUnlocked"": true },
            { ""cardId"": 6, ""quantity"": 1, ""isUnlocked"": true },
            { ""cardId"": 7, ""quantity"": 1, ""isUnlocked"": true }
        ],
        
        ""deckSlots"": [
        { ""ID"": 0, ""currentRarity"": 0 },
        { ""ID"": 0, ""currentRarity"": 1 },
        { ""ID"": 0, ""currentRarity"": 2 },
        { ""ID"": 0, ""currentRarity"": 3 },
        { ""ID"": 0, ""currentRarity"": 3 }
        ]
    }";

    public string GetPlayerCardJson()
    {
        Debug.Log("[MockServer] 카드 데이터 요청 → 더미 JSON 반환");
        return mockJson;
    }

    public void SavePlayerCardJson(string json)
    {
        Debug.Log($"[MockServer] 서버로 저장된 JSON:\n{json}");
    }
}