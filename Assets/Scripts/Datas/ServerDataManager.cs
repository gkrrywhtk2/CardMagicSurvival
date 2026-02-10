using System;
using System.Collections.Generic;
using Game.RankSystem;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    // 싱글톤
    public static ServerDataManager instance;

    // 단일 상태 저장소(root)
    private MockServer mockServer;

    [Header("UI")]
    public TopBar topBar;

    // ✅ 이벤트 (UI 방송용)
    public static event Action<int> OnGoldChanged;           // newGold
    public static event Action<int> OnUpgradeStoneChanged;   // newStone

    // 편의: 항상 root 데이터 참조
    private ServerData Data => mockServer != null ? mockServer.GetData() : null;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Debug.LogWarning("[ServerDataManager] Duplicate instance detected. Destroying this object.");
            Destroy(gameObject);
            return;
        }

        mockServer = GetComponent<MockServer>();
        if (mockServer == null)
        {
            Debug.LogError("[ServerDataManager] MockServer component not found!");
            return;
        }

        EnsureNonNull(Data);

        if (topBar != null)
            topBar.gameObject.SetActive(true);

        // (선택) 처음 UI 동기화 1회
        OnGoldChanged?.Invoke(GetCurrentGold());
        OnUpgradeStoneChanged?.Invoke(GetCurrentUpgradeStone());
    }

    // =========================
    // Null Guard
    // =========================
    private static void EnsureNonNull(ServerData data)
    {
        if (data == null) return;

        data.heroAccounts ??= Array.Empty<HeroAccount>();
        data.accountCards ??= Array.Empty<AccountCard>();
        data.deckSlots ??= Array.Empty<DeckSlot>();
        data.Currency ??= new Currency { Gold = 0, UpgradeStone = 0 };
    }

    // =========================
    // Read APIs (UI/로직은 이것만 봄)
    // =========================


    //Card 관련
    public AccountSpellCard[] GetListOfAccountSpellCards()
    {
        var d = Data;
        EnsureNonNull(d);
        return d.accountSpellCards;
    }
    public List<DeckSlot> GetDeckSlots()
    {
        var d = Data;
        EnsureNonNull(d);
        return new List<DeckSlot>(d.deckSlots);
    }

    public ServerData GetData()
    {
        var d = Data;
        EnsureNonNull(d);
        return d;
    }

    public HeroAccount[] GetHeroAccounts()
    {
        var d = Data;
        EnsureNonNull(d);
        return d.heroAccounts;
    }

    public HeroAccount GetHeroAccount(int heroId)
    {
        var heroes = GetHeroAccounts();
        return Array.Find(heroes, h => h != null && h.heroId == heroId);
    }

    public HeroAccount GetSelectedHero()
    {
        var heroes = GetHeroAccounts();
        return Array.Find(heroes, h => h != null && h.isSelected)
               ?? (heroes.Length > 0 ? heroes[0] : null);
    }

    public int GetSelectedHeroId()
    {
        var h = GetSelectedHero();
        return h != null ? h.heroId : 0;
    }
    public int GetCurrentLevel(int heroId)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return 0;
        }

        return hero.level;
    }

    public int GetCurrentGold()
    {
        var d = Data;
        EnsureNonNull(d);
        return d.Currency.Gold;
    }

    public int GetCurrentUpgradeStone()
    {
        var d = Data;
        EnsureNonNull(d);
        return d.Currency.UpgradeStone;
    }

    // 저장/전송 payload (웹서버 붙이면 이 JSON을 POST)
    public string BuildSavePayloadJson(bool pretty = true)
    {
        return mockServer != null ? mockServer.ExportJson(pretty) : "{}";
    }

    // =========================
    // Write APIs (Data 직접 수정)
    // =========================

    /// <summary>
    /// 선택 영웅 변경 (이벤트 없음: UI는 호출한 쪽에서 Refresh)
    /// </summary>
    public void UpdateSelectedHero(int newSelectedHeroId)
    {
        var d = Data;
        EnsureNonNull(d);

        foreach (var hero in d.heroAccounts)
        {
            if (hero == null) continue;
            hero.isSelected = (hero.heroId == newSelectedHeroId);
        }
    }
    public bool TryChangeSelectedHeroSkillLevel(HeroAccount selectedHero, RankType rank, int delta, int min = 0, int max = 10)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = selectedHero;
        if (hero == null)
        {
            Debug.LogWarning("[ServerDataManager] No selected hero.");
            return false;
        }

        // 1) 현재 레벨 가져오기
        int cur = GetSkillLevelBySlot(hero, rank);

        // 2) 변경 + Clamp
        int next = Mathf.Clamp(cur + delta, min, max);

        // 변화 없으면 종료
        if (next == cur) return false;

        // 3) root(heroAccount)에 직접 세팅 (=> MockServer.root에 바로 반영됨)
        SetSkillLevelBySlot(hero, rank, next);

        Debug.Log($"[ServerDataManager] Selected Hero({hero.heroId}) {rank} skill level: {cur} -> {next}");
        return true;
    }

    private static int GetSkillLevelBySlot(HeroAccount hero, RankType rank)
    {
        return rank switch
        {
            RankType.Uncommon => hero.cSkillLevel,
            RankType.Rare => hero.rSkillLevel,
            RankType.Epic => hero.eSkillLevel,
            RankType.Legendary => hero.lSkillLevel,
            RankType.Mythic => hero.mSkillLevel,
            _ => hero.cSkillLevel
        };
    }

    private static void SetSkillLevelBySlot(HeroAccount hero, RankType rank, int value)
    {
        switch (rank)
        {
            case RankType.Uncommon: hero.cSkillLevel = value; break;
            case RankType.Rare: hero.rSkillLevel = value; break;
            case RankType.Epic: hero.eSkillLevel = value; break;
            case RankType.Legendary: hero.lSkillLevel = value; break;
            case RankType.Mythic: hero.mSkillLevel = value; break;
        }
    }

    

    /// <summary>
    /// 골드 추가/차감 (이벤트 있음)
    /// </summary>
    public void AddGold(int amount)
    {
        var d = Data;
        EnsureNonNull(d);

        int prev = d.Currency.Gold;
        d.Currency.Gold = Mathf.Max(0, d.Currency.Gold + amount);

        Debug.Log($"[ServerDataManager] Gold updated: {prev} -> {d.Currency.Gold} (change: {amount})");
        OnGoldChanged?.Invoke(d.Currency.Gold);
    }

    public void SetGold(int amount)
    {
        var d = Data;
        EnsureNonNull(d);

        int prev = d.Currency.Gold;
        d.Currency.Gold = Mathf.Max(0, amount);

        Debug.Log($"[ServerDataManager] Gold set: {prev} -> {d.Currency.Gold}");
        OnGoldChanged?.Invoke(d.Currency.Gold);
    }

    //영웅 등급 관련

    public void HeroRankUp(int heroId, RankType rank)
    {
        var d = Data;
        EnsureNonNull(d);
        d.heroAccounts[heroId].rank = (int)rank;
    }

    public RankType GetHeroRank(int heroId)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return RankType.Uncommon;
        }

        return (RankType)hero.rank;
    }
    /// <summary>
    /// 강화석 추가/차감 (이벤트 있음)
    /// </summary>
    public void AddUpgradeStone(int amount)
    {
        var d = Data;
        EnsureNonNull(d);

        int prev = d.Currency.UpgradeStone;
        d.Currency.UpgradeStone = Mathf.Max(0, d.Currency.UpgradeStone + amount);

        Debug.Log($"[ServerDataManager] UpgradeStone updated: {prev} -> {d.Currency.UpgradeStone} (change: {amount})");
        OnUpgradeStoneChanged?.Invoke(d.Currency.UpgradeStone);
    }

    public void SetUpgradeStone(int amount)
    {
        var d = Data;
        EnsureNonNull(d);

        int prev = d.Currency.UpgradeStone;
        d.Currency.UpgradeStone = Mathf.Max(0, amount);

        Debug.Log($"[ServerDataManager] UpgradeStone set: {prev} -> {d.Currency.UpgradeStone}");
        OnUpgradeStoneChanged?.Invoke(d.Currency.UpgradeStone);
    }

    public bool GetIsMythicHero(int heroId)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return false;
        }

        return hero.mSkillLevel >= 1;
    }

    /// <summary>
    /// 레벨업 (이벤트 없음: UI는 호출한 쪽에서 Refresh)
    /// </summary>
    /// 
    ///
    public int GetCurrentExp(int heroId)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return 0;
        }

        return hero.exp;
    }

    //카드 관련 로직
    public int GetCardStock(int cardId)
    {
        var d = Data;
        EnsureNonNull(d);

        var card = Array.Find(d.accountSpellCards, c => c != null && c.id == cardId);
        if (card == null)
        {
            Debug.LogError($"[ServerDataManager] Card {cardId} not found!");
            return 0;
        }

        return card.stock;
    }
    public int GetCardLevel(int cardId)
    {
        var d = Data;
        EnsureNonNull(d);

        var card = Array.Find(d.accountSpellCards, c => c != null && c.id == cardId);
        if (card == null)
        {
            Debug.LogError($"[ServerDataManager] Card {cardId} not found!");
            return 0;
        }

        return card.level;
    }
    public bool GetCardUnlocked(int cardId)
    {
        var d = Data;
        EnsureNonNull(d);

        var card = Array.Find(d.accountSpellCards, c => c != null && c.id == cardId);
        if (card == null)
        {
            Debug.LogError($"[ServerDataManager] Card {cardId} not found!");
            return false;
        }

        return card.isUnlocked;
    }

    //카드 관련 로직 끝
    public bool LevelUp(int heroId)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return false;
        }

        if(hero.level >= 20)
            return false;

        hero.level += 1;
        hero.exp = 0;
        return true;
    }

    /// <summary>
    /// 경험치 구매: 골드 소비 + exp 1 증가 (골드 이벤트만 쏨)
    /// </summary>
    public bool BuyExp(int heroId, int maxExp, int GoldCost)
    {
        var d = Data;
        EnsureNonNull(d);

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return false;
        }

        hero.exp += 1;
        AddGold(-GoldCost);


        OnGoldChanged?.Invoke(d.Currency.Gold);
        return true;
    }

    // =========================
    // Debug
    // =========================
    [ContextMenu("Debug: Show Data (Root)")]
        public void DebugShowServerData()
    {
        var d = Data;
        EnsureNonNull(d);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("========== DATA(ROOT) STATUS ==========");

        // Currency
        sb.AppendLine($"\n💰 GOLD: {d.Currency.Gold}");
        sb.AppendLine($"🪨 UPGRADE STONE: {d.Currency.UpgradeStone}");

        // Heroes
        sb.AppendLine("\n👤 HERO ACCOUNTS:");
        if (d.heroAccounts != null && d.heroAccounts.Length > 0)
        {
            foreach (var hero in d.heroAccounts)
            {
                if (hero == null) continue;
                string selected = hero.isSelected ? "⭐ SELECTED" : "";
                string unlocked = hero.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Hero ID: {hero.heroId} | Lv.{hero.level} | Rank: {hero.rank} | Exp: {hero.exp} {selected}");
            }
        }
        else sb.AppendLine("  No heroes found");

        // ✅ Account Spell Cards (최신)
        sb.AppendLine("\n🃏 ACCOUNT SPELL CARDS:");
        if (d.accountSpellCards != null && d.accountSpellCards.Length > 0)
        {
            foreach (var card in d.accountSpellCards)
            {
                // struct면 null 체크 불필요지만 안전하게 둠
                if (card == null) continue;
                string unlocked = card.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Card ID: {card.id} | Lv.{card.level} | Stock: {card.stock} | Unlocked: {card.isUnlocked}");
            }
        }
        else sb.AppendLine("  No accountSpellCards found");

        // ✅ Deck Slots (ID-only)
        sb.AppendLine("\n📋 DECK SLOTS (ID-only):");
        if (d.deckSlots != null && d.deckSlots.Length > 0)
        {
            for (int i = 0; i < d.deckSlots.Length; i++)
            {
                var slot = d.deckSlots[i];
                if (slot == null)
                {
                    sb.AppendLine($"  Slot {i}: (null)");
                    continue;
                }

                sb.AppendLine($"  Slot {i}: Card ID {slot.ID}");
            }
        }
        else sb.AppendLine("  No deck slots found");

        // (선택) legacy accountCards가 남아있다면 비교 출력
        if (d.accountCards != null && d.accountCards.Length > 0)
        {
            sb.AppendLine("\n🧩 LEGACY accountCards (optional):");
            foreach (var c in d.accountCards)
            {
                if (c == null) continue;
                string unlocked = c.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Card ID: {c.cardId} | Quantity: {c.quantity}");
            }
        }

        sb.AppendLine("\n=======================================");
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Debug: Show Raw JSON (Export)")]
    public void DebugShowRawJson()
    {
        Debug.Log("========== RAW(JSON Export) ==========\n" + BuildSavePayloadJson(true) + "\n====================================");
    }

    // =========================
    // DTOs
    // =========================
    [Serializable]
    public class ServerData
    {
        public AccountCard[] accountCards;
        public DeckSlot[] deckSlots;
        public HeroAccount[] heroAccounts;
        public Currency Currency; // ✅ JSON의 "Currency"와 동일
        public AccountSpellCard[] accountSpellCards;
    }

    [Serializable]
    public class Currency
    {
        public int Gold;
        public int UpgradeStone;
    }

    [Serializable]
    public class HeroAccount
    {
        public int heroId;
        public int level;
        public int rank;
        public bool isUnlocked;
        public int exp;
        public bool isSelected;

        // 스킬 레벨
        public int cSkillLevel;
        public int rSkillLevel;
        public int eSkillLevel;
        public int lSkillLevel;
        public int mSkillLevel;

        // 스킬 경험치
        public int cSkillExp;
        public int rSkillExp;
        public int eSkillExp;
        public int lSkillExp;
        public int mSkillExp;
    }

    [Serializable]
    public class DeckSlot
    {
        public int ID;
    }
    [Serializable]
    public class AccountSpellCard
    {
        public int id;
        public int stock;
        public int level;
        public bool isUnlocked;
    }
}
