using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using RANK;
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
    //Upgrade,RandomCard
    private List<Card> candidatePool = new List<Card>(); // 랜덤 등장 카드 풀
    public RandomCard[] randomCard;//랜덤 등장 카드
    public RectTransform[] randomCardPoints;//랜덤 생성 카드 생성 위치
    public RandomCardDescPanel randomCardDescUI;//카드 설명 UI
    public GameObject arrow;//화살표 게임오브젝트

    private void Start()
    {
        CardSelect(); // 초기 덱 선택
        randomCard[0].RandomCardNum = 0;
        randomCard[1].RandomCardNum = 1;
        randomCard[2].RandomCardNum = 2;
       
      
    }
    public void CardSelect()
    {
        // 초기 덱 세팅 (여기서는 0, 1, 2, 3을 예제로 사용)
        deck.Add(new Card(0, 1));
        deck.Add(new Card(1, 1));
        deck.Add(new Card(2, 1));
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


    public void StartUpgradeEvent(){
        UpgradeEvent(deck, cardDatas);
    }

     public void UpgradeEvent(List<Card> deck, CardData[] allCards){
         // 1. 후보 카드 풀 초기화
        UpdateCandidatePool(deck, allCards);

        //2. 후보 카드의 개수
        int candidateCount = Mathf.Min(3, candidatePool.Count);
        List<Card> selectedCards = new List<Card>();
        //3. 랜덤 카드 선택(최대 3장)
    
            for (int i = 0; i < candidateCount; i++)
            {
            int randomIndex = Random.Range(0, candidatePool.Count);
            selectedCards.Add(candidatePool[randomIndex]);
            candidatePool.RemoveAt(randomIndex); // 선택된 카드는 후보 풀에서 임시 제거
            }
        
        
        //selectedCard 개수에 따른 분기, 백그라운드 이미지(어두문 배경) 활성화
        GameManager.instance.backG.SetActive(true);
        switch (selectedCards.Count){
            case 0:
            break;

            case 1:
            randomCard[0].gameObject.SetActive(true);
            randomCard[0].transform.position = randomCardPoints[2].position;
            randomCard[0].Init(selectedCards[0]);
            break;

            case 2:
            randomCard[0].gameObject.SetActive(true);
            randomCard[0].transform.position = randomCardPoints[1].position;
            randomCard[1].gameObject.SetActive(true);
            randomCard[1].transform.position = randomCardPoints[3].position;
            randomCard[0].Init(selectedCards[0]);
            randomCard[1].Init(selectedCards[1]);
            break;

            case 3:
            randomCard[0].gameObject.SetActive(true);
            randomCard[0].transform.position = randomCardPoints[0].position;
            randomCard[1].gameObject.SetActive(true);
            randomCard[1].transform.position = randomCardPoints[2].position;
            randomCard[2].gameObject.SetActive(true);
            randomCard[2].transform.position = randomCardPoints[4].position;

            randomCard[0].Init(selectedCards[0]);
            randomCard[1].Init(selectedCards[1]);
            randomCard[2].Init(selectedCards[2]);

            break;
        }
            int index = 0;//첫번째 randomCard
            RandomCardSelectedSetting(index);


        
     }

    private void UpdateCandidatePool(List<Card> deck, CardData[] allCards)
{
    // 기존 후보 풀 초기화
    candidatePool.Clear();

    // 1. 업그레이드 가능한 카드 추가
    foreach (var card in deck)
    {
        if (card.Rank < 3) // 레벨 3 미만인 카드만 추가
        {
            //기존 카드보다 1레벨 높은 카드를 풀에 추가.
            candidatePool.Add(new Card(card.ID, card.Rank + 1));
        }
    }
    //1-2 업그레이드
    foreach(var handCard in magicCards){
        if(handCard.cardRank < 3){
            candidatePool.Add(new Card(handCard.cardId, handCard.cardRank + 1));
        }
    }

  // 2. 신규 카드 추가
foreach (var cardData in allCards)
{
    // deck(List)와 magicCards(배열)를 모두 검사
    bool isCardInDeckOrHand = deck.Exists(c => c.ID == cardData.cardId) || 
    System.Array.Exists(magicCards, h => h.cardId == cardData.cardId);

    // 덱과 핸드 카드에 없는 카드만 추가
    if (!isCardInDeckOrHand)
    {
        candidatePool.Add(new Card(cardData.cardId, 1)); // 신규 카드는 레벨 1로 추가
    }
}

    Debug.Log($"후보 풀에 {candidatePool.Count}개의 카드가 업데이트되었습니다.");
}

    public void TakeCardInfo(Card card){
        //우선 랜덤 카드 비활성화
        GameManager.instance.backG.SetActive(false);
        randomCard[0].gameObject.SetActive(false);
        randomCard[1].gameObject.SetActive(false);
        randomCard[2].gameObject.SetActive(false);
        //선택된 카드 덱에 추가
        AddCard_ToHand(card);
        //deck.Add(new Card(card.ID, card.Rank));
        GameManager.instance.GamePlayState = true;
        
    }

   public void AddCard_ToHand(Card newCard)
{
    // 1. 덱에서 같은 ID의 카드를 찾음
    var existingCardInDeck = deck.Find(card => card.ID == newCard.ID);

    if (existingCardInDeck != null)
    {
        // 이미 존재하는 카드의 Rank를 올림
        if (existingCardInDeck.Rank < 3)
        {
            existingCardInDeck.Rank += 1;
        }
    }
    else//deck에 카드가 없다면 핸드를 찾아본다.
    {
        var existingCardInHand = System.Array.Find(magicCards, card => card.cardId == newCard.ID);
    Debug.Log(existingCardInHand);
        if (existingCardInHand != null)
        {
        // 이미 존재하는 카드의 Rank를 올림
            if (existingCardInHand.cardRank < 3)
            {
           // existingCardInHand.cardRank = Mathf.Min(existingCardInHand.cardRank + 1, 3);
           // existingCardInHand.RankImageSetting(existingCardInHand.cardRank); // Rank 이미지 업데이트
            existingCardInHand.Init_CardUpgrade(existingCardInHand.cardRank + 1);// 등급 업
            }
        }
        else{
              // 신규 카드라면 덱에 추가
        deck.Add(newCard);
        }
    }
    }

    public void RandomCardSelectedSetting(int index){
        //랜덤 카드 선택 표시

        //초기화
        randomCard[0].outLine.gameObject.SetActive(false);
        randomCard[1].outLine.gameObject.SetActive(false);
        randomCard[2].outLine.gameObject.SetActive(false);

        randomCard[index].outLine.gameObject.SetActive(true);

        //arrow 오브젝트 좌표 세팅
        Vector3 arrowX = randomCard[index].GetComponent<RectTransform>().anchoredPosition;
        arrow.GetComponent<RectTransform>().anchoredPosition = new Vector3(arrowX.x, -173, 0);//arrow 오브젝트 좌표 설정

        //카드 설명Ui 세팅
        randomCardDescUI.gameObject.SetActive(true);
        int ID = randomCard[index].cardId;
        int LEVEL = randomCard[index].cardLevel;
        CardData data = cardDatas[ID];
        randomCardDescUI.UISetting(data, LEVEL);

        
    }
}