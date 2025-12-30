using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.RankSystem;

public class HeroCardManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private MockServer mockServer;
    public HeroCard[] heroCards;

    // ===== JsonUtility용 (매니저 내부 중첩 DTO) =====
    [Serializable]
    private class Root
    {
        public List<HeroAccount> heroAccounts;
    }

    [Serializable]
    private class HeroAccount
    {
        public int heroId;
        public int level;
        public int rank;
        public bool isUnlocked;
        public int exp;
    }

    // 캐시(원하면 제거 가능)
    private string cachedJson;
    private Root cachedRoot;

    // =====================
    // ✅ 기존 기능 (유지)
    // =====================
    public void UpdateAllHeroCardFrames()
    {
        if (mockServer == null)
        {
            Debug.LogError("[HeroCardManager] mockServer is null");
            return;
        }

        string json = mockServer.GetServerJson();
        Root root = GetParsedRoot(json);

        if (root == null || root.heroAccounts == null)
        {
            Debug.LogError("[HeroCardManager] heroAccounts parse failed");
            return;
        }

        // heroId -> HeroAccount
        var map = new Dictionary<int, HeroAccount>();
        foreach (var h in root.heroAccounts)
            map[h.heroId] = h;

        foreach (var card in heroCards)
        {
            if (card == null) continue;

            if (map.TryGetValue(card.heroID, out var dto))
            {
                card.Init(dto.level, dto.exp, dto.rank, dto.isUnlocked);
            }
            else
            {
                // 서버에 없는 heroID면 잠금 기본 처리
                card.Init(level: 0, exp: 0, rankInt: 0, isUnlocked: false);
            }
        }
    }

    private Root GetParsedRoot(string json)
    {
        if (!string.IsNullOrEmpty(cachedJson) && cachedJson == json && cachedRoot != null)
            return cachedRoot;

        cachedJson = json;
        cachedRoot = JsonUtility.FromJson<Root>(json);
        return cachedRoot;
    }

    // =====================
    // ✅ 정렬 기능 (추가)
    // =====================

    // enum 값이 바뀌어도 정렬 기준이 안정적으로 유지되게 매핑
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

    // 등급 오름차 (낮은 등급 -> 높은 등급)
    public void SortByRankAsc()
    {
        heroCards = heroCards
            .Where(c => c != null)
            .OrderByDescending(c => c.ownedBool)   // 보유 먼저 (원치 않으면 삭제)
            .ThenBy(c => RankOrder(c.rank))
            .ThenByDescending(c => c.heroLevel)
            .ThenBy(c => c.heroID)
            .ToArray();

        ApplyUISiblingOrder();
    }

    // 등급 내림차 (높은 등급 -> 낮은 등급)
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

    // 레벨 오름차 (낮은 레벨 -> 높은 레벨)
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

    // 레벨 내림차 (높은 레벨 -> 낮은 레벨)
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

    // UI 순서 반영
    private void ApplyUISiblingOrder()
    {
        for (int i = 0; i < heroCards.Length; i++)
        {
            heroCards[i].transform.SetSiblingIndex(i);
        }
    }
}
