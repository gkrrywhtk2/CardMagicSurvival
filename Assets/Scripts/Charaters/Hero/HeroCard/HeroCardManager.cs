using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.RankSystem;

public class HeroCardManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private ServerDataManager serverDataManager;
    public HeroCard[] heroCards;

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

        // ✅ root 단일 상태에서 바로 읽기
        var serverData = serverDataManager.GetData();
        if (serverData == null || serverData.heroAccounts == null)
        {
            Debug.LogError("[HeroCardManager] serverData or heroAccounts is null");
            return;
        }

        // heroId -> HeroAccount 매핑
        var map = new Dictionary<int, ServerDataManager.HeroAccount>(serverData.heroAccounts.Length);
        foreach (var h in serverData.heroAccounts)
        {
            if (h == null) continue;
            map[h.heroId] = h;
        }

        foreach (var card in heroCards)
        {
            if (card == null) continue;

            if (map.TryGetValue(card.heroID, out var dto) && dto != null)
            {
                card.Init(dto.level, dto.exp, dto.rank, dto.isUnlocked, dto.isSelected);

                // (선택) 카드가 가지고 있는 rank 필드가 따로 있다면 dto 기반으로 동기화해도 됨
                // card.rank = (RankType)dto.rank;
            }
            else
            {
                // 서버에 없는 heroID면 잠금 기본 처리
                card.Init(level: 0, exp: 0, rankInt: 0, isUnlocked: false, isSelected: false);
            }
        }
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
            if (heroCards[i] == null) continue;
            heroCards[i].transform.SetSiblingIndex(i);
        }
    }
}
