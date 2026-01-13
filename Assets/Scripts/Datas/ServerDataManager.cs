using System;
using Unity.VisualScripting;
using UnityEngine;

public class ServerDataManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static ServerDataManager instance;

    // 백엔드 서버와 이어주는 중간 연결다리
    private MockServer mockServer;

    public static event Action<int> OnGoldChanged; // 골드 변경 이벤트

    public TopBar topBar;//탑 네비

     // ✅ 캐시
    public ServerData cachedData { get; private set; }

    private void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
        {
            instance = this;
        }
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
        }
         RefreshFromServer();   // ✅ 시작 시 파싱해서 세팅

        topBar.gameObject.SetActive(true);//세팅 끝난뒤에 네비게이션 호출
    }

    /// <summary>
    /// MockServer JSON을 파싱해서 캐시 갱신
    /// </summary>
    public void RefreshFromServer()
    {
        cachedData = GetParsedServerData();

        if (cachedData == null)
        {
            Debug.LogError("[ServerDataManager] RefreshFromServer failed: cachedData is null");
            return;
        }

        // null 방지
        cachedData.heroAccounts ??= Array.Empty<HeroAccount>();
        cachedData.accountCards ??= Array.Empty<AccountCard>();
        cachedData.deckSlots ??= Array.Empty<DeckSlot>();
        cachedData.Gold ??= new Currency_Gold { Gold = 0 };

        Debug.Log($"[ServerDataManager] RefreshFromServer OK. heroes={cachedData.heroAccounts.Length}");
    }

     /// <summary>
    /// 캐시에 있는 heroAccounts 반환(편의)
    /// </summary>
    public HeroAccount[] GetHeroAccounts()
    {
        if (cachedData == null) RefreshFromServer();
        return cachedData != null ? cachedData.heroAccounts : Array.Empty<HeroAccount>();
    }

    /// <summary>
    /// 선택된 영웅 반환(편의)
    /// </summary>
    public HeroAccount GetSelectedHero()
    {
        var heroes = GetHeroAccounts();
        return Array.Find(heroes, h => h.isSelected) ?? (heroes.Length > 0 ? heroes[0] : null);
    }

    /// <summary>
    /// 서버에서 전체 JSON 데이터 가져오기
    /// </summary>
    public string GetServerJson()
    {
        return mockServer.GetServerJson();
    }

    /// <summary>
    /// 서버 데이터를 파싱하여 ServerData 객체로 반환
    /// </summary>
    public ServerData GetParsedServerData()
    {
        try
        {
            string json = mockServer.GetServerJson();
            return JsonUtility.FromJson<ServerData>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"[ServerDataManager] Parse failed: {e.Message}");
            return null;
        }
    }

    /// <summary>
    /// 선택된 영웅을 서버 데이터에 업데이트
    /// </summary>
    public void UpdateSelectedHero(int newSelectedHeroId)
    {
        if (cachedData == null) RefreshFromServer();
        if (cachedData == null) return;

        foreach (var hero in cachedData.heroAccounts)
            hero.isSelected = false;

        var selectedHero = Array.Find(cachedData.heroAccounts, h => h.heroId == newSelectedHeroId);
        if (selectedHero != null) selectedHero.isSelected = true;

        // 저장
        mockServer.SaveServerJson(JsonUtility.ToJson(cachedData, true));

        // 필요하면 다시 파싱해서 확정(선택)
        // RefreshFromServer();
    }

        /// <summary>
    /// 서버 데이터에서 현재 선택된 영웅 ID 반환
    /// </summary>
    public int GetSelectedHeroId()
    {
        var serverData = ServerDataManager.instance.GetParsedServerData();
        
        if (serverData == null || serverData.heroAccounts == null)
        {
            Debug.LogWarning("[GameManager] Failed to get server data. Using default hero ID 0.");
            return 0;
        }

        var selectedHero = Array.Find(serverData.heroAccounts, h => h.isSelected);
        
        if (selectedHero != null)
        {
            Debug.Log($"[GameManager] Selected hero ID: {selectedHero.heroId}");
            return selectedHero.heroId;
        }
        
        Debug.LogWarning("[GameManager] No hero selected in server data. Using default hero ID 0.");
        return 0;
    }

        /// <summary>
    /// 골드 추가 (양수: 획득, 음수: 소비)
    /// </summary>
    public void AddGold(int amount)
    {
        try
        {
            // 1. 현재 서버 데이터 가져오기
            var data = GetParsedServerData();
            if (data == null)
            {
                Debug.LogError("[ServerDataManager] Failed to get server data");
                return;
            }
            
            // 2. Currency_Gold가 null이면 초기화
            if (data.Gold == null)
            {
                data.Gold = new Currency_Gold { Gold = 0 };
            }
            
            // 3. 골드 추가/차감
            int previousGold = data.Gold.Gold;
            data.Gold.Gold += amount;

             // 이벤트 발생
            OnGoldChanged?.Invoke(data.Gold.Gold);
            
            // 4. 골드가 음수가 되지 않도록 보호
            if (data.Gold.Gold < 0)
            {
                Debug.LogWarning($"[ServerDataManager] Insufficient gold! Current: {previousGold}, Attempted: {amount}");
                data.Gold.Gold = 0;
            }
            
            Debug.Log($"[ServerDataManager] Gold updated: {previousGold} → {data.Gold.Gold} (change: {amount})");
            
            // 5. 수정된 데이터를 다시 서버에 저장
            string updatedJson = JsonUtility.ToJson(data, true);
            mockServer.SaveServerJson(updatedJson);

            // 6. 이벤트 발생 (최종 골드 값으로)
            OnGoldChanged?.Invoke(data.Gold.Gold);
            
        }
        catch (Exception e)
        {
            Debug.LogError($"[ServerDataManager] AddGold failed: {e.Message}");
        }
    }

    /// <summary>
    /// 골드 직접 설정
    /// </summary>
    public void SetGold(int amount)
    {
        try
        {
            // 1. 현재 서버 데이터 가져오기
            var data = GetParsedServerData();
            if (data == null)
            {
                Debug.LogError("[ServerDataManager] Failed to get server data");
                return;
            }
            
            // 2. Currency_Gold가 null이면 초기화
            if (data.Gold == null)
            {
                data.Gold = new Currency_Gold();
            }
            
            // 3. 골드 설정 (음수 방지)
            data.Gold.Gold = Mathf.Max(0, amount);
            
            Debug.Log($"[ServerDataManager] Gold set to: {data.Gold.Gold}");
            
            // 4. 수정된 데이터를 다시 서버에 저장
            string updatedJson = JsonUtility.ToJson(data, true);
            mockServer.SaveServerJson(updatedJson);

            // 6. 이벤트 발생 (최종 골드 값으로)
            OnGoldChanged?.Invoke(data.Gold.Gold);
            
        }
        catch (Exception e)
        {
            Debug.LogError($"[ServerDataManager] SetGold failed: {e.Message}");
        }
    }

    /// <summary>
    /// 현재 골드 조회
    /// </summary>
    public int GetCurrentGold()
    {
        var data = GetParsedServerData();
        
        if (data == null || data.Gold == null)
        {
            Debug.LogWarning("[ServerDataManager] Failed to get gold data. Returning 0.");
            return 0;
        }
        
        return data.Gold.Gold;
    }

    public bool LevelUp(int heroId)
    {
            var data = GetParsedServerData();
            if (data == null || data.heroAccounts == null)
            {
                Debug.LogError("[ServerDataManager] Failed to get server data");
                return false;
            }
             // 2. 해당 영웅 찾기
            var hero = Array.Find(data.heroAccounts, h => h.heroId == heroId);
            if (hero == null)
            {
                Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
                return false;
            }

             //레벨업
            
                hero.level += 1;
                hero.exp = 0;
               // Debug.Log($"[ServerDataManager] Hero {heroId} leveled up to {hero.level}!");
            
             // 서버에 저장
            string updatedJson = JsonUtility.ToJson(data, true);
            mockServer.SaveServerJson(updatedJson);
            return true;
    }


    /// <summary>
    /// 경험치 구매 (골드 소비, 경험치 1 획득)
    /// </summary>
    public bool BuyExp(int heroId, int maxExp, int goldCost = 3000)
    {
        try
        {
            var data = GetParsedServerData();
            if (data == null || data.heroAccounts == null)
            {
                Debug.LogError("[ServerDataManager] Failed to get server data");
                return false;
            }
            
            // Gold가 null이면 초기화
            if (data.Gold == null)
            {
                data.Gold = new Currency_Gold { Gold = 0 };
            }
            
            // 1. 골드 확인
            if (data.Gold.Gold < goldCost)
            {
                Debug.LogWarning($"[ServerDataManager] Not enough gold! Current: {data.Gold.Gold}, Required: {goldCost}");
                return false;
            }
            
            // 2. 해당 영웅 찾기
            var hero = Array.Find(data.heroAccounts, h => h.heroId == heroId);
            if (hero == null)
            {
                Debug.LogError($"[ServerDataManager] Hero {heroId} not found!");
                return false;
            }
            
            // 3. 골드 차감
            data.Gold.Gold -= goldCost;
            
            // 4. 경험치 추가
            hero.exp += 1;

            
            Debug.Log($"[ServerDataManager] Hero {heroId} - Gold: -{goldCost}, Exp: {hero.exp}/{maxExp}, Level: {hero.level}");
            
            // 6. 서버에 저장
            string updatedJson = JsonUtility.ToJson(data, true);
            mockServer.SaveServerJson(updatedJson);
            
            // 7. 골드 이벤트 발생 (최종 골드 값으로)
            OnGoldChanged?.Invoke(data.Gold.Gold);
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[ServerDataManager] BuyExp failed: {e.Message}");
            return false;
        }
    }
    


    // ServerDataManager.cs에 추가

    /// <summary>
    /// 현재 서버 데이터 상태를 콘솔에 출력
    /// </summary>
    [ContextMenu("Debug: Show Server Data")]
    public void DebugShowServerData()
    {
        var data = GetParsedServerData();
        
        if (data == null)
        {
            Debug.LogError("[ServerDataManager] Failed to parse server data!");
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine("========== SERVER DATA STATUS ==========");
        
        // 골드 정보
        sb.AppendLine($"\n💰 GOLD: {(data.Gold != null ? data.Gold.Gold.ToString() : "NULL")}");
        
        // 영웅 정보
        sb.AppendLine("\n👤 HERO ACCOUNTS:");
        if (data.heroAccounts != null && data.heroAccounts.Length > 0)
        {
            foreach (var hero in data.heroAccounts)
            {
                string selected = hero.isSelected ? "⭐ SELECTED" : "";
                string unlocked = hero.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Hero ID: {hero.heroId} | Lv.{hero.level} | Rank: {hero.rank} | Exp: {hero.exp} {selected}");
            }
        }
        else
        {
            sb.AppendLine("  No heroes found");
        }
        
        // 카드 정보
        sb.AppendLine("\n🃏 ACCOUNT CARDS:");
        if (data.accountCards != null && data.accountCards.Length > 0)
        {
            foreach (var card in data.accountCards)
            {
                string unlocked = card.isUnlocked ? "🔓" : "🔒";
                sb.AppendLine($"  {unlocked} Card ID: {card.cardId} | Quantity: {card.quantity}");
            }
        }
        else
        {
            sb.AppendLine("  No cards found");
        }
        
        // 덱 슬롯 정보
        sb.AppendLine("\n📋 DECK SLOTS:");
        if (data.deckSlots != null && data.deckSlots.Length > 0)
        {
            for (int i = 0; i < data.deckSlots.Length; i++)
            {
                var slot = data.deckSlots[i];
                sb.AppendLine($"  Slot {i}: Card ID {slot.ID} | Rarity: {slot.currentRarity}");
            }
        }
        else
        {
            sb.AppendLine("  No deck slots found");
        }
        
        sb.AppendLine("\n========================================");
        
        Debug.Log(sb.ToString());
    }

    /// <summary>
    /// 현재 서버 JSON을 Raw 형태로 출력
    /// </summary>
    [ContextMenu("Debug: Show Raw JSON")]
    public void DebugShowRawJson()
    {
        string json = GetServerJson();
        Debug.Log("========== RAW SERVER JSON ==========\n" + json + "\n====================================");
    }

    // ========== JSON 파싱용 데이터 클래스들 (public으로 공유) ==========
    [Serializable]
    public class ServerData
    {
        public AccountCard[] accountCards;
        public DeckSlot[] deckSlots;
        public HeroAccount[] heroAccounts;
        public Currency_Gold Gold;
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

         // ✅ 스킬 레벨
        public int cSkillLevel;
        public int rSkillLevel;
        public int eSkillLevel;
        public int lSkillLevel;
        public int mSkillLevel;

        //스킬 경험치
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

    [Serializable]
    public class Currency_Gold
    {
        public int Gold;
    }
}