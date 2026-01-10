using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.RankSystem;

public class HeroCardManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ServerDataManager serverDataManager;
    public HeroCard[] heroCards;

    // 캐시 (옵션)
    private string cachedJson;
    private ServerDataManager.ServerData cachedData;

    // =====================
    // ✅ 영웅 카드 업데이트
    // =====================
    public void UpdateAllHeroCardFrames()
    {
        if (serverDataManager == null)
        {
            Debug.LogError("[HeroCardManager] serverDataManager is null");
            return;
        }

        var serverData = GetCachedServerData();

        if (serverData == null || serverData.heroAccounts == null)
        {
            Debug.LogError("[HeroCardManager] heroAccounts parse failed");
            return;
        }

        // heroId -> HeroAccount 매핑
        var map = new Dictionary<int, ServerDataManager.HeroAccount>();
        foreach (var h in serverData.heroAccounts)
            map[h.heroId] = h;

        foreach (var card in heroCards)
        {
            if (card == null) continue;

            if (map.TryGetValue(card.heroID, out var dto))
            {
                card.Init(dto.level, dto.exp, dto.rank, dto.isUnlocked, dto.isSelected);
            }
            else
            {
                // 서버에 없는 heroID면 잠금 기본 처리
                card.Init(level: 0, exp: 0, rankInt: 0, isUnlocked: false, isSelected: false);
            }
        }
    }

    private ServerDataManager.ServerData GetCachedServerData()
    {
        string currentJson = serverDataManager.GetServerJson();
        
        if (!string.IsNullOrEmpty(cachedJson) && cachedJson == currentJson && cachedData != null)
            return cachedData;

        cachedJson = currentJson;
        cachedData = serverDataManager.GetParsedServerData();
        return cachedData;
    }

    // =====================
    // ✅ 정렬 기능
    // =====================

    private int RankOrder(RankType r)
    {
        return r switch
        {
            RankType.Uncommon  => 0,
            RankType.Rare      => 1,
            RankType.Epic      => 2,
            RankType.Legendary => 3,
            RankType.Mythic    => 4,
            _ => 999
        };
    }

    public void SortByRankAsc()
    {
        heroCards = heroCards
            .Where(c => c != null)
            .OrderByDescending(c => c.ownedBool)
            .ThenBy(c => RankOrder(c.rank))
            .ThenByDescending(c => c.heroLevel)
            .ThenBy(c => c.heroID)
            .ToArray();

        ApplyUISiblingOrder();
    }

    public void SortByRankDesc()
    {
        heroCards = heroCards
            .Where(c => c != null)
            .OrderByDescending(c => c.ownedBool)
            .ThenByDescending(c => RankOrder(c.rank))
            .ThenByDescending(c => c.heroLevel)
            .ThenBy(c => c.heroID)
            .ToArray();

        ApplyUISiblingOrder();
    }

    public void SortByLevelAsc()
    {
        heroCards = heroCards
            .Where(c => c != null)
            .OrderByDescending(c => c.ownedBool)
            .ThenBy(c => c.heroLevel)
            .ThenByDescending(c => RankOrder(c.rank))
            .ThenBy(c => c.heroID)
            .ToArray();

        ApplyUISiblingOrder();
    }

    public void SortByLevelDesc()
    {
        heroCards = heroCards
            .Where(c => c != null)
            .OrderByDescending(c => c.ownedBool)
            .ThenByDescending(c => c.heroLevel)
            .ThenByDescending(c => RankOrder(c.rank))
            .ThenBy(c => c.heroID)
            .ToArray();

        ApplyUISiblingOrder();
    }

    private void ApplyUISiblingOrder()
    {
        for (int i = 0; i < heroCards.Length; i++)
        {
            heroCards[i].transform.SetSiblingIndex(i);
        }
    }
}