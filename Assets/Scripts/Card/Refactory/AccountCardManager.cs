using UnityEngine;
using System.Collections.Generic;
using System.Linq;  // ✅ 추가!
using Game.RankSystem;

public class AccountCardManager : MonoBehaviour
{
    public static AccountCardManager Instance { get; private set; }

    public MockServer mockServer;
    
    // ✅ 계정 카드 풀 (보유 현황)
    public List<AccountCard> accountCardPool = new();
    
    // ✅ 덱 슬롯 (PlayerCard로 유지 - 등급 정보 포함)
    public List<PlayerCard> deckSlots = new();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadCardData();
    }

    // ✅ 계정 카드 풀에서 카드 정보 가져오기
    public AccountCard GetAccountCardById(int cardId)
    {
        return accountCardPool.Find(card => card.cardId == cardId);
    }
    
    // ✅ 잠금 해제된 카드만 가져오기
    public List<AccountCard> GetUnlockedCards()
    {
        return accountCardPool.FindAll(card => card.isUnlocked);
    }

    // ✅ 덱 슬롯 ID 리스트 가져오기
    public List<int> GetDeckSlotIds()
    {
        return deckSlots.Select(card => card.ID).ToList();
    }

    public void LoadCardData()
    {
        string json = mockServer.GetPlayerCardJson();
        PlayerCardData data = JsonUtility.FromJson<PlayerCardData>(json);

        accountCardPool.Clear();
        foreach (var card in data.accountCards)
        {
            accountCardPool.Add(card);
        }

        deckSlots.Clear();
        foreach (var card in data.deckSlots)
        {
            deckSlots.Add(card);
        }

        Debug.Log($"[AccountCardManager] 로드 완료 - 보유 카드: {accountCardPool.Count}장, 덱: {deckSlots.Count}장");
    }

    public void SaveCardData()
    {
        PlayerCardData data = new PlayerCardData
        {
            accountCards = accountCardPool.ToArray(),
            deckSlots = deckSlots.ToArray()
        };

        string json = JsonUtility.ToJson(data, true);
        mockServer.SavePlayerCardJson(json);
    }
}

[System.Serializable]
public class PlayerCardData
{
    public AccountCard[] accountCards;  // ✅ 계정 카드 정보
    public PlayerCard[] deckSlots;      // ✅ 덱 슬롯 (등급 포함)
}