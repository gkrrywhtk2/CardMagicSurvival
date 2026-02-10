using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.InGameCardManager;
using Game.RankSystem;
using TMPro;
using Game.CardData;

public class CardSelect : MonoBehaviour
{
    public SelectCard[] selectCards;
    public int currentFocusedCard;
    public InGameCardManager inGameCardManager;

    // ✅ 텍스트 매핑용
    public TMP_Text rank;
    public TMP_Text title;
    public TMP_Text description;

    public void EventStart()
    {
        Time.timeScale = 0;

        // ✅ 계정 카드 전체(merged) 가져오기
        List<ServerDataManager.AccountSpellCard> allCards = AccountCardManager.Instance.mergedCardList;

        // ✅ "보유한 카드만" 풀로 제한
        List<ServerDataManager.AccountSpellCard> ownedCards = allCards
            .Where(c => c.isUnlocked)
            .ToList();

        if (ownedCards.Count == 0)
        {
            Debug.LogWarning("[CardSelect] 보유한 카드가 없습니다. 이벤트를 종료합니다.");
            EventEnd();
            return;
        }

        // ✅ 랜덤으로 3장 뽑기(중복 없이)
        List<ServerDataManager.AccountSpellCard> randomCards = GetRandomCards(ownedCards, 3);

        Debug.Log("<color=#FFFF00>==== 랜덤으로 뽑힌 3장 ====</color>");

        for (int i = 0; i < selectCards.Length; i++)
        {
            // 카드가 3장 미만이면 남는 슬롯은 비활성화(또는 원하는 처리)
            if (i >= randomCards.Count)
            {
                selectCards[i].gameObject.SetActive(false);
                continue;
            }

            selectCards[i].gameObject.SetActive(true);

            int cardId = randomCards[i].id;

            // ✅ 카드 이미지 초기화
            selectCards[i].cardImage.Init(cardId);

            // ✅ 표시/세팅할 레벨 계산
            int displayLevel = GetDisplayLevel(cardId, allCards);

            // ✅ SelectCard에 cardId + cardLevel 저장
            selectCards[i].Init(cardId, displayLevel);

            // ✅ 고정 등급(카드 SO)로 프레임 색상 변경
            RankType fixedRank = CardData.Instance.cardScritableData[cardId].rank;
            selectCards[i].cardImage.UpdateFrameColor(fixedRank);

            Debug.Log($"<color=#00FF00>[Random Card {i}] 카드ID: {cardId}, 표시레벨: {displayLevel}, 고정등급: {fixedRank}</color>");
        }

        // ✅ 포커스 초기화 (가능한 인덱스로)
        currentFocusedCard = Mathf.Clamp(1, 0, randomCards.Count - 1);
        Focus(currentFocusedCard);
    }

    /// <summary>
    /// ✅ 선택지에 표시될 레벨 결정
    /// - 덱(런)에 있으면 RunLevel = deckManage의 LEVEL
    /// - 없으면 AccountLevel = 계정 데이터의 level
    /// </summary>
    private int GetDisplayLevel(int cardId, List<ServerDataManager.AccountSpellCard> allCards)
    {
        // 1) 런 덱에 있으면 RunLevel 사용
        var inDeck = inGameCardManager.deckManage.FirstOrDefault(c => c.ID == cardId);
        if (inDeck != null)
            return inDeck.LEVEL;

        // 2) 덱에 없으면 계정 레벨 사용
        var account = allCards.FirstOrDefault(c => c.id == cardId);
        if (account != null)
            return account.level;

        // 안전장치(원칙상 여기로 오면 이상)
        return 1;
    }

    // ✅ 카드 선택 시 호출
    public void OnCardSelected()
    {
        if (currentFocusedCard < 0 || currentFocusedCard >= selectCards.Length) return;
        if (!selectCards[currentFocusedCard].gameObject.activeSelf) return;

        int selectedCardId = selectCards[currentFocusedCard].cardId;

        ApplySelectResult(selectedCardId);

        Debug.Log($"<color=#00FF00>[CardSelect] 카드 {selectedCardId} 선택 처리 완료</color>");

        EventEnd();
    }

    /// <summary>
    /// ✅ 선택 결과 반영 (덱 중복 불가 전제)
    /// - 덱에 있던 카드면 LEVEL + 1
    /// - 덱에 없던 카드면 계정 level로 PlayerCard 생성 후 deck + deckManage에 추가
    /// </summary>
    private void ApplySelectResult(int cardId)
    {
        // 1) 덱에 이미 있으면 => 레벨업
        var existing = inGameCardManager.deckManage.FirstOrDefault(c => c.ID == cardId);
        if (existing != null)
        {
            existing.LEVEL += 1;
            Debug.Log($"<color=#00FF00>[CardSelect] 기존 카드 {cardId} 런 레벨업 => {existing.LEVEL}</color>");
            return;
        }

        // 2) 덱에 없으면 => 계정 레벨로 추가 (중복 방지 안전 체크 포함)
        if (inGameCardManager.deckManage.Any(c => c.ID == cardId))
        {
            Debug.LogWarning($"[CardSelect] 중복 방지: 카드 {cardId}는 이미 덱에 존재합니다. 추가를 스킵합니다.");
            return;
        }

        var allCards = AccountCardManager.Instance.mergedCardList;
        var account = allCards.FirstOrDefault(c => c.id == cardId);

        int accountLevel = (account != null) ? account.level : 1;

        PlayerCard newCard = new PlayerCard(cardId, accountLevel);

        // ✅ deck 큐 + deckManage에 추가
        inGameCardManager.deck.Enqueue(newCard);
        inGameCardManager.deckManage.Add(newCard);

        Debug.Log($"<color=#00FF00>[CardSelect] 신규 카드 {cardId} 추가 (계정레벨 {accountLevel})</color>");
    }

    // ✅ AccountCard 리스트에서 랜덤 선택(중복 없는 count장)
    private List<ServerDataManager.AccountSpellCard> GetRandomCards(List<ServerDataManager.AccountSpellCard> cardPool, int count)
    {
        if (cardPool.Count <= count)
            return new List<ServerDataManager.AccountSpellCard>(cardPool);

        List<ServerDataManager.AccountSpellCard> shuffled = new List<ServerDataManager.AccountSpellCard>(cardPool);

        // Fisher–Yates 셔플
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        return shuffled.GetRange(0, count);
    }

    public void EventEnd()
    {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void Focus(int index)
    {
        for (int i = 0; i < selectCards.Length; i++)
            selectCards[i].OffFocusCard();

        // ✅ 활성화된 카드 범위 안에서만 포커스
        index = Mathf.Clamp(index, 0, selectCards.Length - 1);
        if (!selectCards[index].gameObject.activeSelf)
        {
            // 활성 슬롯 중 첫 번째로 이동
            int fallback = System.Array.FindIndex(selectCards, c => c != null && c.gameObject.activeSelf);
            if (fallback < 0) return;
            index = fallback;
        }

        currentFocusedCard = index;
        selectCards[currentFocusedCard].OnFocusCard();

        int focusedId = selectCards[currentFocusedCard].cardId;

        // ✅ 카드 이름/설명 매핑
        title.text = CardData.Instance.cardScritableData[focusedId].cardName;
        description.text = CardData.Instance.cardScritableData[focusedId].cardDesc_Main;

        // ✅ 고정 등급 텍스트/색상 매핑 (SO의 rank 사용)
        RankType fixedRank = CardData.Instance.cardScritableData[focusedId].rank;
        rank.text = RankDatas.GetRankString(fixedRank);
        rank.color = RankDatas.GetColor(fixedRank);

        // 필요하면 레벨 표시도 추가 가능 (현재 TMP 변수 없음)
        // Debug.Log($"[Focus] 카드ID: {focusedId}, 표시레벨: {selectCards[currentFocusedCard].cardLevel}");
    }
}
