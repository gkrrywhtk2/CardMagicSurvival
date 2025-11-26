using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.InGameCardManager;
using RANK;
using Game.RankSystem;
using TMPro;
using Game.CardData;

public class CardSelect : MonoBehaviour
{
    public SelectCard[] selectCards;
    public int currentFocusedCard;
    public InGameCardManager inGameCardManager;
    
    // ✅ 이번 이벤트의 등급 (3장 모두 동일)
    private RankType eventRank;

    // ✅ 텍스트 매핑용
    public TMP_Text rank;
    public TMP_Text title;
    public TMP_Text description;

    public void EventStart()
    {
        Time.timeScale = 0;
        
        // ✅ 잠금 해제된 계정 카드만 가져오기
        List<AccountCard> allCards = AccountCardManager.Instance.GetUnlockedCards();
        Debug.Log($"[CardSelect] 보유 카드 수: {allCards.Count}개");

        // ✅ 이벤트 등급 결정 (3장 모두 이 등급으로)
        eventRank = GetRandomRank();
        Debug.Log($"<color=#FFFF00>==== 이벤트 등급: {eventRank} ====</color>");

        // ✅ 랜덤으로 3장 뽑기
        List<AccountCard> randomCards = GetRandomCards(allCards, 3);
        
        Debug.Log("<color=#FFFF00>==== 랜덤으로 뽑힌 3장 ====</color>");
        
        for (int i = 0; i < randomCards.Count; i++)
        {
            int cardId = randomCards[i].cardId;
            
            // ✅ 카드 이미지 초기화
            selectCards[i].cardImage.Init(cardId);

            // ✅ SelectCard 초기화 (cardId와 등급 저장)
            selectCards[i].Init(cardId, eventRank);
        
            // ✅ 모두 동일한 eventRank로 프레임 색상 변경
            selectCards[i].cardImage.UpdateFrameColor(eventRank);
            
            Debug.Log($"<color=#00FF00>[Random Card {i}] 카드ID: {cardId}, 등급: {eventRank}</color>");
        }

        currentFocusedCard = 1;
        Focus(currentFocusedCard);
    }
    
    // ✅ 등급 랜덤 결정 (노말 40%, 레어 30%, 에픽 20%, 레전더리 10%)
    private RankType GetRandomRank()
    {
        float randomValue = Random.Range(0f, 100f);
        
        if (randomValue < 40f) // 40%
        {
            return RankType.Uncommon;
        }
        else if (randomValue < 70f) // 30% (40 + 30)
        {
            return RankType.Rare;
        }
        else if (randomValue < 90f) // 20% (70 + 20)
        {
            return RankType.Epic;
        }
        else // 10% (90 ~ 100)
        {
            return RankType.Legendary;
        }
    }
    
    // ✅ 카드 선택 시 호출
    public void OnCardSelected()
    {
        int selectedCardId = selectCards[currentFocusedCard].cardId;
        
        // ✅ 선택한 카드를 이벤트 등급으로 덱에 추가
        AddCardToDeck(selectedCardId, eventRank);
        
        Debug.Log($"<color=#00FF00>[CardSelect] 카드 {selectedCardId} (등급: {eventRank})를 덱에 추가했습니다!</color>");
        
        EventEnd();
    }
    
    // ✅ 덱에 카드 추가 (특정 등급으로)
    private void AddCardToDeck(int cardId, RankType rank)
    {
        // ✅ 새로운 PlayerCard 생성 (등급 지정)
        PlayerCard newInGameCard = new PlayerCard(cardId, rank);
        
        // 1. 덱 큐에 추가
        inGameCardManager.deck.Enqueue(newInGameCard);
        
        // 2. deckManage에도 추가
        inGameCardManager.deckManage.Add(newInGameCard);
        
        // 3. 전체 핸드 새로고침 필요없어짐
       // inGameCardManager.RefreshAllHand();
        
        Debug.Log($"<color=#00FF00>[CardSelect] 카드 {cardId} (등급: {rank}) 인게임 덱에 추가 완료!</color>");
    }
    
    // ✅ AccountCard 리스트에서 랜덤 선택
    private List<AccountCard> GetRandomCards(List<AccountCard> cardPool, int count)
    {
        if (cardPool.Count < count)
        {
            Debug.LogWarning($"[CardSelect] 카드풀({cardPool.Count}장)이 요청된 개수({count}장)보다 적습니다!");
            return new List<AccountCard>(cardPool);
        }

        List<AccountCard> shuffled = new List<AccountCard>(cardPool);
        
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
        for(int i = 0; i < selectCards.Length; i++)
        {
            selectCards[i].OffFocusCard();
        }
        currentFocusedCard = index;
        selectCards[currentFocusedCard].OnFocusCard();

        // ✅ 카드 이름 매핑
        title.text = CardData.Instance.cardScritableData[selectCards[currentFocusedCard].cardId].cardName;
        
        // ✅ 카드 설명 매핑
        description.text = CardData.Instance.cardScritableData[selectCards[currentFocusedCard].cardId].cardDesc_Main;
        
        // ✅ 카드 등급 텍스트 및 색상 매핑
        RankType currentRank = selectCards[currentFocusedCard].rank;
        rank.text = RankDatas.GetRankString(currentRank);
        rank.color = RankDatas.GetColor(currentRank);
    }
}