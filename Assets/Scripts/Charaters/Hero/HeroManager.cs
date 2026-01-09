using System;
using System.Collections.Generic;
using UnityEngine;
using Game.RankSystem;

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }

    [Header("Refs")]
    public MockServer mockServer;
    public HeroInfo heroInfo;

    [Header("Hero ScriptableObjects")]
    public HeroScriptableObject[] heroes;   // ✅ 배열

    private Dictionary<int, HeroScriptableObject> heroMap;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    void Start()
    {
        BuildMap();
    }

  private void BuildMap()
{
    heroMap = new Dictionary<int, HeroScriptableObject>();

    Debug.Log($"[HeroManager] heroes length={(heroes==null? -1: heroes.Length)}", this); // ✅ return 전에

    if (heroes == null) return;

    foreach (var hero in heroes)
    {
        if (hero == null) continue;

        if (heroMap.ContainsKey(hero.heroId))
        {
            Debug.LogError($"[HeroManager] Duplicate heroId: {hero.heroId}", this);
            continue;
        }

        heroMap.Add(hero.heroId, hero);
    }

    int nullCount = 0;
    foreach (var h in heroes) if (h == null) nullCount++;
    Debug.Log($"[HeroManager] null heroes count={nullCount}", this);
    Debug.Log($"[HeroManager] heroMap count={heroMap.Count}", this);
}


    public HeroScriptableObject GetHeroSO(int heroId)
    {
        if (heroMap == null) BuildMap();

        if (heroMap.TryGetValue(heroId, out var hero))
            return hero;

        Debug.LogError($"[HeroManager] HeroSO not found. heroId={heroId}", this);
        return null;
    }

        public bool ApplyHeroToPlayer(int heroId)
    {
        if (mockServer == null)
        {
            Debug.LogError("[HeroManager] mockServer is null", this);
            return false;
        }

        Player_Main player = GameManager.instance.player;
        if (player == null)
        {
            Debug.LogError("[HeroManager] GameManager.instance.player is null", this);
            return false;
        }

        var heroSO = GetHeroSO(heroId);
        if (heroSO == null) return false;

        var acc = LoadHeroAccountFromServer(heroId);
        if (acc == null)
        {
            Debug.LogError($"[HeroManager] heroAccount not found. heroId={heroId}", this);
            return false;
        }
        if (!acc.isUnlocked)
        {
            Debug.LogWarning($"[HeroManager] hero locked. heroId={heroId}", this);
            return false;
        }

        var rankType = (RankType)acc.rank;

        var calc = new ProgressionV2Calculator(heroSO);
        var stats = calc.GetStats(rankType, acc.level);

        // ✅ Player_Main에 base로 저장만 (heroSO 인자 제거)
        player.ApplyHeroStats(heroId, acc.level, rankType, acc.exp, stats);
        player.autoAttackManager.Apply(heroId, rankType);

                // animator 적용(있으면)
        if (heroSO.animatorController != null)
        {
            var visual = player.GetComponent<PlayerVisual>();
            if (visual != null)
                visual.ApplyController(heroSO.animatorController);
            else
                Debug.LogWarning("[HeroManager] PlayerVisual not found on player.", this);
        }

        return true;
    }

        public bool ApplyHeroToHeroInfo(int heroId)
    {
        if (mockServer == null)
        {
            Debug.LogError("[HeroManager] mockServer is null", this);
            return false;
        }

        Player_Main player = GameManager.instance.player;
        if (player == null)
        {
            Debug.LogError("[HeroManager] GameManager.instance.player is null", this);
            return false;
        }

        var heroSO = GetHeroSO(heroId);
        if (heroSO == null) return false;

        var acc = LoadHeroAccountFromServer(heroId);
        if (acc == null)
        {
            Debug.LogError($"[HeroManager] heroAccount not found. heroId={heroId}", this);
            return false;
        }
        
        if (!acc.isUnlocked)
        {
            Debug.LogWarning($"[HeroManager] hero locked. heroId={heroId}", this);
            return false;
        }

        var rankType = (RankType)acc.rank;
        heroInfo.Init(heroId, acc.level, acc.exp, rankType, acc.isSelected);
        heroInfo.RefreshUI();
        
        return true;
    }

    public int MaxExpSetting(int level)
    {
         return level + 4; //1레벨에 필요량 5, 2레벨에 필요량 6 ...
    }


    // -------- MockServer JSON 읽기 --------
    [Serializable] private class ServerData { public System.Collections.Generic.List<HeroAccount> heroAccounts; }
    [Serializable] private class HeroAccount
    {
        public int heroId;
        public int level;
        public int rank;
        public bool isUnlocked;
        public int exp;
        public bool isSelected;
    }

    private HeroAccount LoadHeroAccountFromServer(int heroId)
    {
        string json = mockServer.GetServerJson();
        var data = JsonUtility.FromJson<ServerData>(json);

        if (data == null || data.heroAccounts == null) return null;

        for (int i = 0; i < data.heroAccounts.Count; i++)
            if (data.heroAccounts[i].heroId == heroId)
                return data.heroAccounts[i];

        return null;
    }
}
