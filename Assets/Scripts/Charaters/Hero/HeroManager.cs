using System.Collections.Generic;
using UnityEngine;
using Game.RankSystem;

public class HeroManager : MonoBehaviour
{
    public static HeroManager Instance { get; private set; }

    [Header("Refs")]
    public HeroInfo heroInfo;

    [Header("Hero ScriptableObjects")]
    public HeroScriptableObject[] heroes; // ✅ 배열
    public AutoAttackManager autoAttackManager;

    private Dictionary<int, HeroScriptableObject> heroMap;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        BuildMap();
    }

    private void BuildMap()
    {
        heroMap = new Dictionary<int, HeroScriptableObject>();

        Debug.Log($"[HeroManager] heroes length={(heroes == null ? -1 : heroes.Length)}", this);

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
        if (heroMap == null || heroMap.Count == 0) BuildMap();

        if (heroMap != null && heroMap.TryGetValue(heroId, out var hero))
            return hero;

        Debug.LogError($"[HeroManager] HeroSO not found. heroId={heroId}", this);
        return null;
    }

    /// <summary>
    /// 플레이어에 영웅 적용 (root 단일 상태: ServerDataManager 기준)
    /// </summary>
    public bool ApplyHeroToPlayer(int heroId)
    {
        if (ServerDataManager.instance == null)
        {
            Debug.LogError("[HeroManager] ServerDataManager.instance is null", this);
            return false;
        }

        Player_Main player = GameManager.instance != null ? GameManager.instance.player : null;
        if (player == null)
        {
            Debug.LogError("[HeroManager] GameManager.instance.player is null", this);
            return false;
        }

        var heroSO = GetHeroSO(heroId);
        if (heroSO == null) return false;

        // ✅ root에서 영웅 계정 가져오기
        var acc = ServerDataManager.instance.GetHeroAccount(heroId);
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

        // ✅ Player_Main에 base로 저장만
        player.ApplyHeroStats(heroId, acc.level, rankType, acc.exp, stats);

        // ✅ 스킬 레벨 적용
        if (player.autoAttackManager != null)
        {
            player.autoAttackManager.Apply(
                heroId,
                rankType,
                acc.cSkillLevel,
                acc.rSkillLevel,
                acc.eSkillLevel,
                acc.lSkillLevel,
                acc.mSkillLevel
            );
        }

        // ✅ animator 적용(있으면)
        if (heroSO.animatorController != null)
        {
            var visual = player.GetComponent<PlayerVisual>();
            if (visual != null) visual.ApplyController(heroSO.animatorController);
            else Debug.LogWarning("[HeroManager] PlayerVisual not found on player.", this);
        }

        return true;
    }

    /// <summary>
    /// HeroInfo 패널에 영웅 적용 (root 단일 상태)
    /// </summary>
    public bool ApplyHeroToHeroInfo(int heroId)
    {
        if (ServerDataManager.instance == null)
        {
            Debug.LogError("[HeroManager] ServerDataManager.instance is null", this);
            return false;
        }

        var heroSO = GetHeroSO(heroId);
        if (heroSO == null) return false;

        var acc = ServerDataManager.instance.GetHeroAccount(heroId);
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

        // ✅ HeroInfo는 RefreshUI에서 root 기준으로 최종 덮어씀
        heroInfo.Init(heroId, acc.level, acc.exp, (RankType)acc.rank, acc.isSelected);
        heroInfo.RefreshUI();

        return true;
    }

    public int MaxExpSetting(int level)
    {
        return level + 4; // 1레벨 필요량 5, 2레벨 필요량 6 ...
    }
        public int MaxUpgradeExpSetting(RankType rank)
    {
        // Uncommon -> Rare : 5
        // Rare -> Epic     : 10
        // Epic -> Legendary: 15
        // Legendary -> Mythic: 20
        // Mythic은 다음 랭크가 없으니 필요량 0(또는 int.MaxValue로 막기)

        if (rank >= RankType.Mythic)
            return 0;

        return ((int)rank + 1) * 5;
    }
}
