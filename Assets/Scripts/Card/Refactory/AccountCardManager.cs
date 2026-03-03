using UnityEngine;
using System.Collections.Generic;
using System.Linq;  // ✅ 추가!
using Game.RankSystem;

public class AccountCardManager : MonoBehaviour
{
    public static AccountCardManager Instance { get; private set; }
    private const int DefaultDeckSlotCount = 5;

    public MockServer mockServer;
    
    // ✅ 계정 카드 풀 (보유 현황)

    public List<ServerDataManager.AccountSpellCard> mergedCardList = new(); // ✅ 병합된 카드 리스트(서버 + 로컬)

    // ✅ 덱 슬롯 (PlayerCard로 유지 - 등급 정보 포함)
    public List<PlayerCard> deckSlots = new();
    public List<ServerDataManager.DeckSlot> accountDeckSlots = new();//서버에서 불러온 덱 슬롯 정보, 처리전, UI용
    public List<ServerDataManager.DeckSlot> cachedCompleteDeckSlots = new();

   private void Awake()
{
    if (Instance == null)
        Instance = this;
    else
        Destroy(gameObject);
}
    private void Start()
    {
        LoadCardData();
    }

    // ✅ 계정 카드 풀에서 카드 정보 가져오기
  
    
    // ✅ 잠금 해제된 카드만 가져오기
    public List<ServerDataManager.AccountSpellCard> GetUnlockedCards()
    {
        return mergedCardList.FindAll(card => card.isUnlocked);
    }

    // ✅ 덱 슬롯 ID 리스트 가져오기
    public List<int> GetDeckSlotIds()
    {
        if (accountDeckSlots != null && accountDeckSlots.Count > 0)
        {
            return accountDeckSlots.Select(slot => slot.ID).ToList();
        }

        if (deckSlots != null && deckSlots.Count > 0)
        {
            return deckSlots.Select(card => card.ID).ToList();
        }

        return new List<int>();
    }

    public void LoadCardData()
    {
        mergedCardList.Clear();
        mergedCardList = BuildMergedCardList();
        accountDeckSlots = CloneDeckSlots(ServerDataManager.instance.GetDeckSlots());
        TryCacheCurrentDeckSlots(accountDeckSlots.Count > 0 ? accountDeckSlots.Count : DefaultDeckSlotCount);
    }
        public List<ServerDataManager.AccountSpellCard> BuildMergedCardList()//서버의 카드 데이터와 로컬의 카드 데이터를 병합
    {
        // 서버 저장된 카드(없을 수도 있음)
        var accountSpellCards = ServerDataManager.instance.GetListOfAccountSpellCards();

        // 게임에 존재하는 모든 카드(기본값 0)
        var allCards = LocalDataManager.Instance.cardData.cardScritableData;

        // 1) 전체 카드 기본 리스트 생성 + id로 빠르게 찾을 딕셔너리 구축
        var mergedList = new List<ServerDataManager.AccountSpellCard>(allCards.Length);
        var byId = new Dictionary<int, ServerDataManager.AccountSpellCard>(allCards.Length);

        foreach (var card in allCards)
        {
            var entry = new ServerDataManager.AccountSpellCard
            {
                id = card.cardId,
                stock = 0,
                level = 1
            };
            mergedList.Add(entry);
            byId[entry.id] = entry;
        }

        // 2) 서버 데이터로 덮어쓰기 (있으면 업데이트, 없으면 무시)
        foreach (var saved in accountSpellCards)
        {
            if (byId.TryGetValue(saved.id, out var target))
            {
                target.id = saved.id;
                target.stock = saved.stock;
                target.level = saved.level;
                target.isUnlocked = saved.isUnlocked;
            }
            // else: 서버에만 있고 로컬에 없는 카드(삭제된 카드 등) -> 무시
            // 필요하면 따로 로그 찍거나 보관 가능
        }

        return mergedList;
    }
    public int GetRequiredCardsForLevelUp(int level)
    {
        // 안전장치 (레벨 1 미만 방지)
        level = Mathf.Max(1, level);

        // 1레벨 5개, 공차 3
        return 5 + (level - 1) * 3;
    }

    public bool TryCacheCurrentDeckSlots(int requiredSlotCount = DefaultDeckSlotCount)
    {
        if (!IsDeckComplete(requiredSlotCount))
        {
            return false;
        }

        cachedCompleteDeckSlots = CloneDeckSlots(accountDeckSlots);
        return true;
    }

    public bool RestoreCachedDeckSlotsIfNeeded(int requiredSlotCount = DefaultDeckSlotCount)
    {
        if (IsDeckComplete(requiredSlotCount))
        {
            return false;
        }

        if (cachedCompleteDeckSlots == null || cachedCompleteDeckSlots.Count != requiredSlotCount)
        {
            Debug.LogWarning("[AccountCardManager] No valid cached deck slots to restore.");
            return false;
        }

        accountDeckSlots = CloneDeckSlots(cachedCompleteDeckSlots);
        SyncDeckSlotsToServerData();
        return true;
    }

    public void SyncDeckSlotsToServerData()
    {
        if (ServerDataManager.instance == null)
        {
            return;
        }

        var data = ServerDataManager.instance.GetData();
        if (data == null)
        {
            return;
        }

        data.deckSlots = CloneDeckSlots(accountDeckSlots).ToArray();
    }

    public bool IsDeckComplete(int requiredSlotCount = DefaultDeckSlotCount)
    {
        if (accountDeckSlots == null || accountDeckSlots.Count != requiredSlotCount)
        {
            return false;
        }

        return accountDeckSlots.All(slot => slot != null && slot.ID != -1);
    }

    private static List<ServerDataManager.DeckSlot> CloneDeckSlots(List<ServerDataManager.DeckSlot> source)
    {
        if (source == null)
        {
            return new List<ServerDataManager.DeckSlot>();
        }

        return source.Select(slot => new ServerDataManager.DeckSlot
        {
            ID = slot != null ? slot.ID : -1
        }).ToList();
    }

}
