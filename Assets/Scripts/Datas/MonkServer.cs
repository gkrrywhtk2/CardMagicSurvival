using UnityEngine;

public class MockServer : MonoBehaviour
{
    private string mockJson = @"
    {
        ""cards"": [
        { ""cardId"": 0, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 1, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 2, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 3, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 4, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 5, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 6, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false },
        { ""cardId"": 7, ""currentRarity"": ""Uncommon"", ""quantity"": 1, ""islocked"": false }
    ],
    
    ""deckSlots"": [0, 1, 2, 3, 4]
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
