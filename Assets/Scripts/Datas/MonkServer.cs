using System;
using UnityEngine;

public class MockServer : MonoBehaviour
{
    [TextArea(10, 60)] private string mockJson = @"
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
        ""cSkillLevel"": 1, ""rSkillLevel"": 1, ""eSkillLevel"": 1, ""lSkillLevel"": 10, ""mSkillLevel"": 0,
        ""cSkillExp"": 3, ""rSkillExp"": 3, ""eSkillExp"": 3, ""lSkillExp"": 3, ""mSkillExp"": 3
        }
    ],
    ""Currency"": { ""Gold"": 9999999, ""UpgradeStone"": 9999999 }
    }";

    // ✅ 인게임에서 쓰는 "진짜 데이터(객체)" = Single Source of Truth
    private ServerDataManager.ServerData root;

    private void Awake()
    {
        LoadFromMockJsonIfNeeded();
    }

    private void LoadFromMockJsonIfNeeded()
    {
        if (root != null) return;

        try
        {
            root = JsonUtility.FromJson<ServerDataManager.ServerData>(mockJson);
        }
        catch (Exception e)
        {
            Debug.LogError($"[MockServer] Failed to parse mockJson: {e.Message}");
            root = null;
        }

        EnsureRootValid();
        Debug.Log("[MockServer] root loaded from mockJson");
    }

    private void EnsureRootValid()
    {
        if (root == null) root = new ServerDataManager.ServerData();

        root.heroAccounts ??= Array.Empty<ServerDataManager.HeroAccount>();
        root.accountCards ??= Array.Empty<ServerDataManager.AccountCard>();
        root.deckSlots ??= Array.Empty<ServerDataManager.DeckSlot>();

        // ✅ Currency null 방지
        root.Currency ??= new ServerDataManager.Currency { Gold = 0, UpgradeStone = 0 };
    }

    // ✅ 인게임 로직은 이걸로 객체를 직접 가져다 씀
    public ServerDataManager.ServerData GetData()
    {
        LoadFromMockJsonIfNeeded();
        return root;
    }

    // ✅ 저장/전송 시점에만 JSON으로 굽기
    public string ExportJson(bool pretty = true)
    {
        LoadFromMockJsonIfNeeded();
        return JsonUtility.ToJson(root, pretty);
    }

    // (호환) 기존 코드가 GetServerJson을 호출해도 문제 없게 유지
    public string GetServerJson()
    {
        return ExportJson(true);
    }

    // (호환) 기존 코드가 SaveServerJson을 호출하면 root도 같이 갱신
    public void SaveServerJson(string json)
    {
        mockJson = json;

        try
        {
            root = JsonUtility.FromJson<ServerDataManager.ServerData>(mockJson);
        }
        catch (Exception e)
        {
            Debug.LogError($"[MockServer] SaveServerJson parse failed: {e.Message}");
            // 실패해도 기존 root 유지 or 새로 만들지 선택 가능
        }

        EnsureRootValid();
    }
}
