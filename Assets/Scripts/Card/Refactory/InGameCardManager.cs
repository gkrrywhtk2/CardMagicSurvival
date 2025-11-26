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
            // ✅ AccountCardManager에서 덱 슬롯 가져오기 (PlayerCard 리스트)
            List<PlayerCard> deckSlots = accountCardManager.deckSlots;
            
            // ✅ 인게임용 복사본 생성
            InitDeckManage(deckSlots);
            
            // ✅ deck 큐 초기화 (deckManage 복사)
            deck = new Queue<PlayerCard>(deckManage);
            
            ShuffleDeck();
            HandInit();
        }

        // ✅ 인게임 덱 관리 리스트 초기화
        private void InitDeckManage(List<PlayerCard> deckSlots)
        {
            deckManage.Clear();
            
            foreach (PlayerCard card in deckSlots)
            {
                // ✅ 인게임용 복사본 생성 (깊은 복사)
                PlayerCard inGameCard = new PlayerCard(card.cardId, card.currentRarity);
                
                deckManage.Add(inGameCard);
                Debug.Log($"[InitDeckManage] 카드 {card.cardId} 추가 - 초기 등급: {inGameCard.currentRarity}");
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
                nextcardImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[nextCard.cardId].cardImage;
            }
        }

        // ✅ 인게임에서 특정 카드의 등급 가져오기
        public RankType GetInGameCardRarity(int cardId)
        {
            PlayerCard card = deckManage.Find(c => c.cardId == cardId);
            return card != null ? card.currentRarity : RankType.Uncommon;
        }

        // ✅ 인게임에서 특정 카드의 등급 업그레이드
        public void UpgradeInGameCardRarity(int cardId)
        {
            PlayerCard card = deckManage.Find(c => c.cardId == cardId);
            if (card != null)
            {
                if (card.currentRarity < RankType.Legendary)
                {
                    RankType oldRarity = card.currentRarity;
                    card.currentRarity = (RankType)((int)card.currentRarity + 1);
                    
                    Debug.Log($"<color=#FFD700>[InGameCardManager] 카드 {cardId} 등급 상승: {oldRarity} → {card.currentRarity}</color>");
                }
                else
                {
                    Debug.Log($"<color=#FF6B6B>[InGameCardManager] 카드 {cardId}는 이미 최대 등급(Legendary)입니다!</color>");
                }
            }
            else
            {
                Debug.LogWarning($"[InGameCardManager] 카드 {cardId}를 deckManage에서 찾을 수 없습니다!");
            }
        }
    }
}