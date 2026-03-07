using System.Linq;
using UnityEngine;
using Game.RankSystem;
using Game.CardData;
using UnityEngine.UI;

public enum SortKey { Rank, Level }
public enum SortDirection { Asc, Desc }

public class UICardManager : MonoBehaviour
{
    // ✅ 싱글톤 인스턴스
    public static UICardManager instance;
    public SpellCard_Button[] spellCard_Buttons;
    public SortButton[] sortButtons; // Rank 버튼 1개, Level 버튼 1개 연결

    [Header("Current Sort State")]
    public SortKey currentSortKey = SortKey.Rank;
    public SortDirection currentSortDirection = SortDirection.Asc;
    public CardInfoManager cardInfoManager;

    [Header("SpellCards_Deck")]
    public SpellCard_Deck[] spellCard_Decks;
    public Image backGroundFocusImage;// ✅ 덱 슬롯 선택 시 배경 이미지 활성화

    private void Awake()
    {
        // ✅ 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        // ✅ 화면이 켜질 때마다 현재 상태를 화살표에 반영
        // (Init 전에 불려도 상관없게)
        RefreshSortUI();
        InitDecks();
    }

    private void OnDisable()
    {
        if (AccountCardManager.Instance == null || spellCard_Decks == null)
        {
            return;
        }

        AccountCardManager.Instance.RestoreCachedDeckSlotsIfNeeded(spellCard_Decks.Length);
    }

    public void InitSpellCardButtons()
    {
        for (int i = 0; i < spellCard_Buttons.Length; i++)
            spellCard_Buttons[i].Init(i);

        // ✅ 처음 진입: 등급 오름차순으로 "실제 정렬 + 화살표 반영"까지 강제
        ApplySort(SortKey.Rank, SortDirection.Asc);
    }

    // =========================
    // Toggle Sort Buttons (2개)
    // =========================

    public void OnClickRankSort()
    {
        if (currentSortKey == SortKey.Rank)
        {
            // 같은 키면 방향 토글
            currentSortDirection = Toggle(currentSortDirection);
        }
        else
        {
            // 다른 키였다면 Rank로 전환, 기본 Asc
            currentSortKey = SortKey.Rank;
            currentSortDirection = SortDirection.Asc;
        }

        ApplySort(currentSortKey, currentSortDirection);
    }

    public void OnClickLevelSort()
    {
        if (currentSortKey == SortKey.Level)
        {
            currentSortDirection = Toggle(currentSortDirection);
        }
        else
        {
            currentSortKey = SortKey.Level;
            currentSortDirection = SortDirection.Asc;
        }

        ApplySort(currentSortKey, currentSortDirection);
    }

    // =========================
    // Core Sort
    // =========================

    private void ApplySort(SortKey key, SortDirection dir)
    {
        currentSortKey = key;
        currentSortDirection = dir;

        if (key == SortKey.Rank)
        {
            // Rank 기준: tie -> Level ↓ -> Id ↑
            spellCard_Buttons = (dir == SortDirection.Asc)
                ? spellCard_Buttons
                    .OrderBy(b => GetRank(b.cardId))
                    .ThenByDescending(b => GetLevel(b.cardId))
                    .ThenBy(b => b.cardId)
                    .ToArray()
                : spellCard_Buttons
                    .OrderByDescending(b => GetRank(b.cardId))
                    .ThenByDescending(b => GetLevel(b.cardId))
                    .ThenBy(b => b.cardId)
                    .ToArray();
        }
        else // Level
        {
            // Level 기준: tie -> Rank ↓ -> Id ↑
            spellCard_Buttons = (dir == SortDirection.Asc)
                ? spellCard_Buttons
                    .OrderBy(b => GetLevel(b.cardId))
                    .ThenByDescending(b => GetRank(b.cardId))
                    .ThenBy(b => b.cardId)
                    .ToArray()
                : spellCard_Buttons
                    .OrderByDescending(b => GetLevel(b.cardId))
                    .ThenByDescending(b => GetRank(b.cardId))
                    .ThenBy(b => b.cardId)
                    .ToArray();
        }

        ApplySiblingOrder();
        RefreshSortUI();
    }

    private SortDirection Toggle(SortDirection d)
        => d == SortDirection.Asc ? SortDirection.Desc : SortDirection.Asc;

    private RankType GetRank(int cardId)
        => CardData.Instance.cardScritableData[cardId].rank;

    private int GetLevel(int cardId)
        => ServerDataManager.instance.GetCardLevel(cardId);

    private void ApplySiblingOrder()
    {
        for (int i = 0; i < spellCard_Buttons.Length; i++)
            spellCard_Buttons[i].transform.SetSiblingIndex(i);
    }

    private void RefreshSortUI()
    {
        if (sortButtons == null) return;
        foreach (var b in sortButtons)
        {
            if (b == null) continue;
            b.ApplyState(currentSortKey, currentSortDirection);
        }
    }

    // =========================
    // Deck 관련 (초기화, 덱 슬롯 클릭)
    // =========================
    public void InitDecks()
    {
        if (spellCard_Decks == null || spellCard_Decks.Length == 0) return;

        var decklist = AccountCardManager.Instance != null
            ? AccountCardManager.Instance.GetDeckSlotIds()
            : null;

        if (decklist == null || decklist.Count == 0)
        {
            Debug.LogWarning("[UICardManager] decklist is empty. Skip deck UI init.");
            return;
        }

        int initCount = Mathf.Min(spellCard_Decks.Length, decklist.Count);
        if (decklist.Count < spellCard_Decks.Length)
        {
            Debug.LogWarning($"[UICardManager] deck slot count mismatch. UI:{spellCard_Decks.Length}, Data:{decklist.Count}");
        }

        for (int i = 0; i < initCount; i++)
        {
            if (spellCard_Decks[i] == null) continue;
            spellCard_Decks[i].slotId = i;
            spellCard_Decks[i].Init(decklist[i]);
        }

        AccountCardManager.Instance?.TryCacheCurrentDeckSlots(spellCard_Decks.Length);
    }
    public void OnClickBackGroundFocus()
    {
        InitDecks();
        backGroundFocusImage.gameObject.SetActive(false);//배경 원상복귀
    }
}
