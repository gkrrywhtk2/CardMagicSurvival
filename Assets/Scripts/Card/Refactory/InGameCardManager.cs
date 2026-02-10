using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;
using Unity.VisualScripting;
using Game.RankSystem;

namespace Game.InGameCardManager
{
    public class InGameCardManager : MonoBehaviour
    {
        public AccountCardManager accountCardManager;
        public Queue<PlayerCard> deck = new Queue<PlayerCard>();  // ✅ PlayerCard 큐
        public List<PlayerCard> deckManage = new();

        public MagicCard[] magicCards;
        public Image nextcardImage;

        public void InitDeck()
    {
        // 1) 서버 덱 슬롯 가져오기 (ID-only, 5개 고정)
        var serverSlots = accountCardManager.accountDeckSlots ?? new List<ServerDataManager.DeckSlot>();

        // 2) 계정 카드 레벨 맵: id -> level (unlock만)
        var accountCards = accountCardManager.mergedCardList; // List<ServerDataManager.AccountSpellCard>
        var levelMap = accountCards
            .Where(c => c.isUnlocked)
            .ToDictionary(c => c.id, c => c.level);

        // 3) 서버 슬롯(ID) -> PlayerCard(ID, 계정레벨)로 변환
        List<PlayerCard> inGameDeckSlots = new List<PlayerCard>();

        foreach (var s in serverSlots)
        {
            int id = s.ID;

            // 계정 레벨 없으면 기본 1 (덱은 항상 유효하게)
            if (!levelMap.TryGetValue(id, out int accountLevel))
            {
                Debug.LogWarning($"[InitDeck] 카드ID {id} 계정 레벨 없음(잠금/불일치). 기본 레벨 1로 진행");
                accountLevel = 1;
            }

            inGameDeckSlots.Add(new PlayerCard(id, accountLevel));
            Debug.Log($"[InitDeck] 카드 {id} 시작 레벨(계정): {accountLevel}");
        }

        // 4) 인게임 관리 리스트(깊은 복사)
        InitDeckManage(inGameDeckSlots);

        // 5) 큐 구성
        deck = new Queue<PlayerCard>(deckManage);

        ShuffleDeck();
        HandInit();
    }

    // ✅ 인게임 덱 관리 리스트 초기화(깊은 복사)
    private void InitDeckManage(List<PlayerCard> deckSlots)
    {
        deckManage.Clear();

        foreach (PlayerCard card in deckSlots)
        {
            PlayerCard inGameCard = new PlayerCard(card.ID, card.LEVEL);
            deckManage.Add(inGameCard);
            Debug.Log($"[InitDeckManage] 카드 {card.ID} 추가 - 초기 레벨: {inGameCard.LEVEL}");
        }

        Debug.Log($"<color=#00FFFF>[InitDeckManage] 인게임 덱 관리 초기화 완료 - 총 {deckManage.Count}장</color>");
    }


        public void ShuffleDeck()
        {
            // ✅ Queue<PlayerCard>를 List로 변환
            List<PlayerCard> tempList = deck.ToList();

            // Fisher–Yates 알고리즘으로 섞기
            for (int i = tempList.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                (tempList[i], tempList[randomIndex]) = (tempList[randomIndex], tempList[i]);
            }

            // ✅ 다시 Queue<PlayerCard>로 변환
            deck = new Queue<PlayerCard>(tempList);

            Debug.Log($"[ShuffleDeck] 덱 셔플 완료 → {deck.Count}장");
        }

        public void HandInit()
        {
            StartCoroutine(FirstDraw());
        }

        private IEnumerator FirstDraw()
        {
            int handCount = 4;

            magicCards[0].CardReload();
            magicCards[1].CardReload();
            magicCards[2].CardReload();
            magicCards[3].CardReload();

            for (int i = 0; i < handCount; i++)
            {
                // ✅ PlayerCard 가져오기
                PlayerCard card = deck.Dequeue();
                
                // ✅ PlayerCard 객체 전달
                magicCards[i].CardInit(card);

                yield return new WaitForSeconds(0.3f);

            }

            NextCardImageSetting();
        }

        public void DrawCard(int handNumber)
        {
            StartCoroutine(DrawCard_Corutine(handNumber));
        }

        public IEnumerator DrawCard_Corutine(int handNumber)
        {
            // ✅ PlayerCard 가져오기
            PlayerCard card = deck.Dequeue();

            magicCards[handNumber].CardFalse();

            int basicDrawCoolTime = 1;
            yield return new WaitForSeconds(basicDrawCoolTime);

            // ✅ PlayerCard 객체 전달
            magicCards[handNumber].CardInit(card);
            NextCardImageSetting();
        }

        // ✅ 전체 핸드 새로고침 -> 필요없어짐
        // public void RefreshAllHand()
        // {
        //     for (int i = 0; i < magicCards.Length; i++)
        //     {
        //         CardImage cardImage = magicCards[i].GetComponent<CardImage>();
                
        //         if (cardImage != null && cardImage.cardId != 0)
        //         {
        //             RankType currentRank = GetInGameCardRarity(cardImage.cardId);
        //             cardImage.UpdateFrameColor(currentRank);
                    
        //             Debug.Log($"<color=#00FFFF>[RefreshAllHand] 핸드 슬롯 {i} 업데이트 → 카드ID: {cardImage.cardId}, 등급: {currentRank}</color>");
        //         }
        //     }
        // }

        public void NextCardImageSetting()
        {
            if (deck.Count > 0)
            {
                // ✅ PlayerCard를 Peek
                PlayerCard nextCard = deck.Peek();
                nextcardImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[nextCard.ID].cardImage;
            }
        }
    }
}