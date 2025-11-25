using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Game.InGameCardManager;
using RANK;
using Game.RankSystem;

public class CardSelect : MonoBehaviour
{
    public SelectCard[] selectCards;
    public int currentFocusedCard;
    public InGameCardManager inGameCardManager;

    public void EventStart()
    {
        Time.timeScale = 0;
        
        List<PlayerCard> allCards = AccountCardManager.Instance.GetAccountCardPool();
        Debug.Log($"[CardSelect] 보유 카드 수: {allCards.Count}개");

        // ✅ 인게임에서 이미 Legendary인 카드를 제외하고 랜덤 선택
        List<PlayerCard> randomCards = GetRandomCardsExcludingMaxRarity(allCards, 3);
        
        Debug.Log("<color=#FFFF00>==== 랜덤으로 뽑힌 3장 ====</color>");
        
        for (int i = 0; i < randomCards.Count; i++)
        {
            int cardId = randomCards[i].cardId;
            
            selectCards[i].cardImage.Init(cardId);
            
            if (IsCardInCurrentDeck(cardId))
            {
                RankType currentInGameRank = inGameCardManager.GetInGameCardRarity(cardId);
                
                // ✅ 다음 등급 표시 (Legendary가 최대)
                if (currentInGameRank < RankType.Legendary)
                {
                    RankType nextRank = (RankType)((int)currentInGameRank + 1);
                    selectCards[i].cardImage.UpdateFrameColor(nextRank);
                    
                    Debug.Log($"<color=#FFD700>[Random Card {i}] 카드ID: {cardId} - 현재: {currentInGameRank} → 업그레이드 시: {nextRank}</color>");
                }
            }
            else
            {
                Debug.Log($"<color=#00FF00>[Random Card {i}] 카드ID: {cardId}, 계정 등급: {randomCards[i].currentRarity}</color>");
            }
        }

        currentFocusedCard = 1;
        Focus(currentFocusedCard);
    }
    
    // ✅ 인게임에서 이미 Legendary인 카드를 제외하고 랜덤 선택
    private List<PlayerCard> GetRandomCardsExcludingMaxRarity(List<PlayerCard> cardPool, int count)
    {
        // ✅ 인게임 덱에서 이미 Legendary인 카드 ID 목록
        List<int> legendaryCardIds = inGameCardManager.deckManage
            .Where(card => card.currentRarity >= RankType.Legendary)
            .Select(card => card.cardId)
            .ToList();
        
        // ✅ Legendary 카드 제외한 풀
        List<PlayerCard> filteredPool = cardPool
            .Where(card => !legendaryCardIds.Contains(card.cardId))
            .ToList();
        
        Debug.Log($"[CardSelect] 전체 카드: {cardPool.Count}장, 선택 가능: {filteredPool.Count}장, 제외된 Legendary: {legendaryCardIds.Count}장");
        
        if (filteredPool.Count < count)
        {
            Debug.LogWarning($"[CardSelect] 선택 가능한 카드({filteredPool.Count}장)가 요청된 개수({count}장)보다 적습니다!");
            return GetRandomCards(filteredPool, filteredPool.Count);
        }

        return GetRandomCards(filteredPool, count);
    }
    
    private List<PlayerCard> GetRandomCards(List<PlayerCard> cardPool, int count)
    {
        if (cardPool.Count < count)
        {
            return new List<PlayerCard>(cardPool);
        }

        List<PlayerCard> shuffled = new List<PlayerCard>(cardPool);
        
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        return shuffled.GetRange(0, count);
    }
    
    public void OnCardSelected()
    {
        //선택한 카드 덱에 추가
        int selectedCardId = selectCards[currentFocusedCard].cardImage.cardId;
        
        bool isInDeck = IsCardInCurrentDeck(selectedCardId);
        
        if (isInDeck)
        {
            inGameCardManager.UpgradeInGameCardRarity(selectedCardId);
            
            // ✅ 전체 핸드 새로고침
            inGameCardManager.RefreshAllHand();
            
            Debug.Log($"<color=#FFD700>[CardSelect] 카드 {selectedCardId} 업그레이드 및 전체 핸드 새로고침 완료!</color>");
        }
        else
        {
            AddCardToDeck(selectedCardId);
            Debug.Log($"<color=#00FF00>[CardSelect] 카드 {selectedCardId}를 덱에 추가했습니다!</color>");
        }
        
        EventEnd();
    }
    private bool IsCardInCurrentDeck(int cardId)
    {
        return inGameCardManager.deckManage.Exists(card => card.cardId == cardId);
    }
    
    private void AddCardToDeck(int cardId)
    {
        inGameCardManager.deck.Enqueue(cardId);
        
        PlayerCard originalCard = AccountCardManager.Instance.GetCardById(cardId);
        if (originalCard != null)
        {
            PlayerCard newInGameCard = new PlayerCard(cardId)
            {
                currentRarity = originalCard.currentRarity,
                quantity = originalCard.quantity,
                islocked = originalCard.islocked
            };
            
            inGameCardManager.deckManage.Add(newInGameCard);
            Debug.Log($"<color=#00FF00>[CardSelect] 카드 {cardId}를 인게임 덱에 추가 완료!</color>");
        }
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
    }
}