using System;
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

    /// <summary>
    /// 레벨업 (이벤트 없음: UI는 호출한 쪽에서 Refresh)
    /// </summary>
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

        hero.level += 1;
        hero.exp = 0;
        return true;
    }

    /// <summary>
    /// 경험치 구매: 골드 소비 + exp 1 증가 (골드 이벤트만 쏨)
    /// </summary>
    public bool BuyExp(int heroId, int maxExp, int goldCost = 3000)
    {
        var d = Data;
        EnsureNonNull(d);

        if (d.Currency.Gold < goldCost)
        {
            Debug.LogWarning($"[ServerDataManager] Not enough gold! Current: {d.Currency.Gold}, Required: {goldCost}");
            return false;
        }

        var hero = Array.Find(d.heroAccounts, h => h != null && h.heroId == heroId);
        if (hero == null)
        {
            Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
            return false;
        }

        d.Currency.Gold -= goldCost;
        hero.exp += 1;

        Debug.Log($"[ServerDataManager] Hero {heroId} - Gold: -{goldCost}, Exp: {hero.exp}/{maxExp}, Level: {hero.level}");

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

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("========== DATA(ROOT) STATUS ==========");
        sb.AppendLine($"\n💰 GOLD: {d.Currency.Gold}");
        sb.AppendLine($"🪨 UPGRADE STONE: {d.Currency.UpgradeStone}");

        sb.AppendLine("\n👤 HERO ACCOUNTS:");
        if (d.heroAccounts.Length > 0)
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

        sb.AppendLine("\n🃏 ACCOUNT CARDS:");
        if (d.accountCards.Length > 0)
        {
            foreach (var card in d.accountCards)
            {
                if (card == null) continue;
                string unlocked = card.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Card ID: {card.cardId} | Quantity: {card.quantity}");
            }
        }
        else sb.AppendLine("  No cards found");

        sb.AppendLine("\n📋 DECK SLOTS:");
        if (d.deckSlots.Length > 0)
        {
            for (int i = 0; i < d.deckSlots.Length; i++)
            {
                var slot = d.deckSlots[i];
                if (slot == null) continue;
                sb.AppendLine($"  Slot {i}: Card ID {slot.ID} | Rarity: {slot.currentRarity}");
            }
        }
        else sb.AppendLine("  No deck slots found");

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
    public class AccountCard
    {
        public int cardId;
        public int quantity;
        public bool isUnlocked;
    }

    [Serializable]
    public class DeckSlot
    {
        public int ID;
        public int currentRarity;
    }
}
