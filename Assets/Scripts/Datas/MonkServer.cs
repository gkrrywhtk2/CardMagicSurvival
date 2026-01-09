using UnityEngine;

public class MockServer : MonoBehaviour
{
    [TextArea(10, 60)]
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
            { ""ID"": 1, ""currentRarity"": 0 },
            { ""ID"": 1, ""currentRarity"": 1 },
            { ""ID"": 2, ""currentRarity"": 2 },
            { ""ID"": 2, ""currentRarity"": 3 },
            { ""ID"": 1, ""currentRarity"": 3 }
        ],   
        ""heroAccounts"": [
            { ""heroId"": 0, ""level"": 3, ""rank"": 2, ""isUnlocked"": true,  ""exp"": 3, ""isSelected"": true },
            { ""heroId"": 1, ""level"": 20, ""rank"": 3, ""isUnlocked"": true,  ""exp"": 0, ""isSelected"": false }
        ]
    }";

    // ✅ 서버에서 "전체 JSON" 내려주는 느낌
    public string GetServerJson()
    {
        Debug.Log("[MockServer] GetServerJson()");
        return mockJson;
    }

    // ✅ 서버에 "전체 JSON" 저장하는 느낌
    public void SaveServerJson(string json)
    {
        mockJson = json;
        Debug.Log($"[MockServer] SaveServerJson() len={json?.Length ?? 0}");
    }
}
