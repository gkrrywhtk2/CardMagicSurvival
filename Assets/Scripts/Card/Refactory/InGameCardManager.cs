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
{   //역할
    // 1. 게임중 덱을 관리 
    public AccountCardManager accountCardManager; // 참조 연결
    public Queue<int> deck = new Queue<int>();    // 인게임 덱
    public List<PlayerCard> deckManage = new(); // 인게임 덱 관리, 카드 전체보기 현재 카드의 등급(인게임에서 달라지니까)

    //오브젝트
    public MagicCard[] magicCards; //카드 오브젝트
    public Image nextcardImage;//다음 카드 이미지


    public void InitDeck()
    {
        // 1. AccountCardManager에서 현재 선택된 덱 슬롯 가져오기
        List<int> deckIds = accountCardManager.GetDeckSlotIds();
        
        // 2. 덱 ID 큐 초기화
        deck = new Queue<int>(deckIds);
        
        // ✅ 3. deckManage 초기화 - 계정 카드 정보의 인게임 복사본 생성
        InitDeckManage(deckIds);
        
        ShuffleDeck();
        HandInit();
    }

    // ✅ 인게임 덱 관리 리스트 초기화
        private void InitDeckManage(List<int> deckIds)
        {
            deckManage.Clear();
            
            foreach (int cardId in deckIds)
            {
                // 계정에서 해당 카드 정보 가져오기
                PlayerCard originalCard = accountCardManager.GetCardById(cardId);
                
                if (originalCard != null)
                {
                    // ✅ 인게임용 복사본 생성 (깊은 복사)
                    PlayerCard inGameCard = new PlayerCard(cardId)
                    {
                        currentRarity = originalCard.currentRarity,
                        quantity = originalCard.quantity,
                        islocked = originalCard.islocked
                    };
                    
                    deckManage.Add(inGameCard);
                    Debug.Log($"[InitDeckManage] 카드 {cardId} 추가 - 초기 등급: {inGameCard.currentRarity}");
                }
                else
                {
                    Debug.LogWarning($"[InitDeckManage] 카드 ID {cardId}를 찾을 수 없습니다!");
                }
            }
            
            Debug.Log($"<color=#00FFFF>[InitDeckManage] 인게임 덱 관리 초기화 완료 - 총 {deckManage.Count}장</color>");
        }


    public void ShuffleDeck()
    {
        // Queue를 List로 변환
        List<int> tempList = deck.ToList();

        // Fisher–Yates 알고리즘으로 섞기
        for (int i = tempList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (tempList[i], tempList[randomIndex]) = (tempList[randomIndex], tempList[i]);
        }

        // 다시 Queue로 변환
        deck = new Queue<int>(tempList);

        Debug.Log($"[ShuffleDeck] 덱 셔플 완료 → {string.Join(", ", deck)}");
    }

    public void HandInit()
    {
        //덱을 설정한 후 처음 3장 뽑기 
        StartCoroutine(FirstDraw());
    }

    private IEnumerator FirstDraw()
    {

        // **2. 덱에서 4장 핸드로
        int handCount = 4;

        //카드 리로딩 애니메이션
        magicCards[0].CardReload();
        magicCards[1].CardReload();
        magicCards[2].CardReload();
        magicCards[3].CardReload();

        for (int i = 0; i < handCount; i++)
        {
            int cardId = deck.Dequeue(); // 덱 맨 위의 카드 ID 가져오기
           // deck.Enqueue(cardId); //덱 맨위로 올리기

            // 핸드 카드 초기화
            magicCards[i].CardInit(cardId);

            // 0.3초 지연시켜서 순서대로 뽑게끔
            yield return new WaitForSeconds(0.3f);
        }

        // 마지막에 다음 카드 이미지 설정
            NextCardImageSetting();
    }

    public void DrawCard(int handNumber)
    {
        //핸드에서 카드를 사용한 후 드로우하는 함수
        StartCoroutine(DrawCard_Corutine(handNumber));
    }

    public IEnumerator DrawCard_Corutine(int handNumber)
    {
        int cardId = deck.Dequeue(); // 우선 순위 카드 ID 가져오기
        //deck.Enqueue(cardId);  //맨위로 올리기

        magicCards[handNumber].CardFalse();//카드 숨기기

        //드로우 딜레이
        int basicDrawCoolTime = 1;
        yield return new WaitForSeconds(basicDrawCoolTime); // 1초 기다림

        //카드 데이터 세팅
        magicCards[handNumber].CardInit(cardId);
        NextCardImageSetting();
    }

        // ✅ 카드 업그레이드 했을 때 전체 핸드 현제 인게임덱에 맞게 새로고침
    public void RefreshAllHand()
    {
        for (int i = 0; i < magicCards.Length; i++)
        {
            CardImage cardImage = magicCards[i].GetComponent<CardImage>();
            
            if (cardImage != null && cardImage.cardId != 0)
            {
                RankType currentRank = GetInGameCardRarity(cardImage.cardId);
                cardImage.UpdateFrameColor(currentRank);
                
                Debug.Log($"<color=#00FFFF>[RefreshAllHand] 핸드 슬롯 {i} 업데이트 → 카드ID: {cardImage.cardId}, 등급: {currentRank}</color>");
            }
        }
    }
    public void NextCardImageSetting()
    {
        int nextCardId = deck.Peek(); // 맨 앞 요소 '참조만'
        nextcardImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[nextCardId].cardImage;

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
            // ✅ 일반 카드의 최대 등급은 Legendary (Mythic은 특별 카드 전용)
            if (card.currentRarity < RankType.Legendary)
            {
                RankType oldRarity = card.currentRarity;
                card.currentRarity = (RankType)((int)card.currentRarity + 1);
                
                Color newColor = RankDatas.GetColor(card.currentRarity);
                
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
