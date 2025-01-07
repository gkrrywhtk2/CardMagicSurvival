using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    public CardData[] cardDatas;         // 모든 카드 데이터
    public List<int> deck = new List<int>(); // 현재 덱
    public RectTransform[] boardPoints; // 카드 보드 포인트
    public MagicCard[] magicCards;      // 핸드 카드 총 3장
    public Image nextcardImage;//다음 카드 이미지

    public void CardSelect()
    {
        // 초기 덱 세팅 (여기서는 0, 1, 2, 3을 예제로 사용)
        deck.Add(0);
        deck.Add(1);
        deck.Add(2);
        deck.Add(3);
         deck.Add(4);
    }

    public void DeckSetting()
    {
        // **1. Fisher-Yates 알고리즘을 사용해 덱 리스트를 섞음**
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }

        // **2. 덱에서 3장 뽑아 핸드에 배치**
        int handCount = 3;

        for (int i = 0; i < handCount; i++)
        {
                int cardId = deck[0]; // 덱 맨 위의 카드 ID 가져오기
                deck.RemoveAt(0);    // 덱 맨 위의 카드 제거

                // 핸드 카드 초기화
                //Debug.Log(cardId);
                magicCards[i].CardInit(cardDatas[cardId]);
        
        }
        NextCardImageSetting();
    }
    public void DrawCard(int fixedCard){
                int cardId = deck[0]; // 덱 맨 위의 카드 ID 가져오기
                deck.RemoveAt(0);    // 덱 맨 위의 카드 제거

                // 핸드 카드 초기화
                magicCards[fixedCard].CardInit(cardDatas[cardId]);
                NextCardImageSetting();
    }

    public void NextCardImageSetting(){
        nextcardImage.sprite = cardDatas[deck[0]].nextcardImage;
    }

    private void Start()
    {
        CardSelect(); // 초기 덱 선택
        DeckSetting(); // 덱 셋팅 및 핸드 배치
    }
}