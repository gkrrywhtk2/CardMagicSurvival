using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class Card
{
    public int ID { get; set; } // 카드 ID
    public int Rank { get; set; } // 카드 레벨

    public Card(int id, int rank)
    {
        ID = id;
        Rank = rank;
    }
}

public class DeckManager : MonoBehaviour
{
    public CardData[] cardDatas;         // 모든 카드 데이터
    public List<Card> deck = new List<Card>(); // 현재 덱
    public RectTransform[] boardPoints; // 카드 보드 포인트
    public MagicCard[] magicCards;      // 핸드 카드 총 3장
    public Image nextcardImage;//다음 카드 이미지


    private void Start()
    {
        CardSelect(); // 초기 덱 선택
      
    }
    public void CardSelect()
    {
        // 초기 덱 세팅 (여기서는 0, 1, 2, 3을 예제로 사용)
        deck.Add(new Card(0, 1));
          deck.Add(new Card(1, 2));
            deck.Add(new Card(2, 3));
              deck.Add(new Card(3, 1));
                deck.Add(new Card(4, 1));
       
    }

    public void DeckSetting()
{
    StartCoroutine(DeckSettingCoroutine());
}

private IEnumerator DeckSettingCoroutine()
{
    // Fisher-Yates 알고리즘으로 덱 섞기
for (int i = deck.Count - 1; i > 0; i--)
{
    int randomIndex = Random.Range(0, i + 1);

    // Swap entire objects, not just IDs
    Card temp = deck[i];
    deck[i] = deck[randomIndex];
    deck[randomIndex] = temp;
}

    // **2. 덱에서 3장 뽑아 핸드에 배치**
    int handCount = 3;

    for (int i = 0; i < handCount; i++)
    {
        int cardId = deck[0].ID; // 덱 맨 위의 카드 ID 가져오기
        int cardLevel = deck[0].Rank;
        deck.RemoveAt(0);    // 덱 맨 위의 카드 제거

        // 핸드 카드 초기화
        magicCards[i].CardInit(cardDatas[cardId], cardLevel);

        // 0.3초 지연
        yield return new WaitForSeconds(0.3f);
    }

    // 마지막에 다음 카드 이미지 설정
    NextCardImageSetting();
}
    public void DrawCard(int fixedCard){
                int cardId = deck[0].ID; // 덱 맨 위의 카드 ID 가져오기
                int cardLevel = deck[0].Rank;
                deck.RemoveAt(0);    // 덱 맨 위의 카드 제거

                // 핸드 카드 초기화
                magicCards[fixedCard].CardInit(cardDatas[cardId], cardLevel);
                NextCardImageSetting();
    }

    public void NextCardImageSetting(){
        nextcardImage.sprite = cardDatas[deck[0].ID].nextcardImage;
    }

  
}