using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


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
        {
        ""heroId"": 0, ""level"": 3, ""rank"": 3, ""isUnlocked"": true, ""exp"": 3, ""isSelected"": true,
        ""cSkillLevel"": 10, ""rSkillLevel"": 10, ""eSkillLevel"": 10, ""lSkillLevel"": 10, ""mSkillLevel"": 0,
        ""cSkillExp"": 3, ""rSkillExp"": 3, ""eSkillExp"": 3, ""lSkillExp"": 3, ""mSkillExp"": 3
        },
        {
        ""heroId"": 1, ""level"": 20, ""rank"": 0, ""isUnlocked"": true, ""exp"": 0, ""isSelected"": false,
        ""cSkillLevel"": 1, ""rSkillLevel"": 1, ""eSkillLevel"": 1, ""lSkillLevel"": 11, ""mSkillLevel"": 0,
        ""cSkillExp"": 3, ""rSkillExp"": 3, ""eSkillExp"": 3, ""lSkillExp"": 3, ""mSkillExp"": 3
        }
    ],

    ""Gold"": { ""Gold"": 9999999 }
    }";
    //M스킬 레벨이 0이어야 버그 안남. 0,1 이 신화 종자인지 판단

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
