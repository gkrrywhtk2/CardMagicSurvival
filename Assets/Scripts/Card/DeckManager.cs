using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;


public class Card
{
    public int ID { get; set; } // 카드 ID
    public int STACK { get; set; } // 카드 레벨

    public int COUNT { get; set; }// 카드 보유량

    public Card(int id, int stack, int count)
    {
        ID = id;
        STACK = stack;
        COUNT = count;
    }
}

public class DeckManager : MonoBehaviour
{
    public CardData[] cardDatas;         // 모든 카드 데이터
    public List<int> deck = new List<int>(); // 현재 덱 *주의* 카드가 핸드에 있을때는 deck리스트에서 제외됨
    public RectTransform[] boardPoints; // 카드 보드 포인트
    public MagicCard[] magicCards;      // 핸드 카드 총 3장
    public Image nextcardImage;//다음 카드 이미지
    //Upgrade,RandomCard
    private List<Card> candidatePool = new List<Card>(); // 랜덤 등장 카드 풀
    public RandomCard[] randomCard;//랜덤 등장 카드
    public RectTransform[] randomCardPoints;//랜덤 생성 카드 생성 위치
    public RandomCardDescPanel randomCardDescUI;//카드 설명 UI
    public GameObject arrow;//화살표 게임오브젝트
    //CardDescUI
    public GameObject CardDescUi;
    public TMP_Text CardDesc_CardName;
    public TMP_Text CardDesc_CardDesc;
     public List<Card> ownedCardList = new List<Card>(); //
     public GameObject ownedCardset;//덱 관리에서 보유한 카드목록이 풀링되는 부모 오브젝트
     public GameObject[] touchedCard;//카드 정보 오브젝트
     //CardBoardUI
     public CardSetting_UI cardSetting_UI;
    public TMP_Text battleDeckText;//전투덱 n/8
    public RectTransform cardSettingScroll_Rect;
    public RectTransform cardCollectionScroll_Rect;
    public Image[] card_clockBack;//카드 로딩중 카드 보드 360도 쿨타임 연출
    public PreSet_Deck[] preSet_Deck;
    public TMP_Text text_manaAverage;//마나 평균 텍스트

    private void Start()
    {
        //CardSelect(); // 초기 덱 선택
       /// randomCard[0].RandomCardNum = 0;
       // randomCard[1].RandomCardNum = 1;
       // randomCard[2].RandomCardNum = 2;
       
      
    }
    public void GetSavedDeck(List<int> savedDeck)
    {
        // 초기 덱 세팅 (-1값 제거)
        deck.Clear();//초기화
        for(int i = 0; i <savedDeck.Count; i++){
            if(savedDeck[i] != -1){//카드 아이디가 -1인 값은 제외하고 deck에 추가
                deck.Add(savedDeck[i]);
            }
        }

    }

    public void HandSetting()
{
    //덱을 설정한 후 처음 3장 뽑기 
    StartCoroutine(HandSettingCoroutine());
}

private IEnumerator HandSettingCoroutine()
{
    // Fisher-Yates 알고리즘으로 덱 섞기
for (int i = deck.Count - 1; i > 0; i--)
{
    int randomIndex = Random.Range(0, i + 1);

    // Swap entire objects, not just IDs
    int temp = deck[i];
    deck[i] = deck[randomIndex];
    deck[randomIndex] = temp;
}

    // **2. 덱에서 3장 뽑아 핸드에 배치**
    int handCount = 3;

    //카드 리로딩
    magicCards[0].CardReload();
    magicCards[1].CardReload();
    magicCards[2].CardReload();

    // 모든 카드 딜레이 완료될 때까지 대기
    yield return StartCoroutine(ClockBackGroundAnim(1, 0, 1, 2));

    for (int i = 0; i < handCount; i++)
    {
        int cardId = deck[0]; // 덱 맨 위의 카드 ID 가져오기
        int cardStack =  GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == deck[0]).STACK;
        //Card newCard =  GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == deck[0]);
       // int cardLevel = deck[0].STACK;
        deck.RemoveAt(0);    // 덱 맨 위의 카드 제거



        // 핸드 카드 초기화
        magicCards[i].CardInit(cardId);

        // 0.3초 지연
        yield return new WaitForSeconds(0.3f);
    }

    // 마지막에 다음 카드 이미지 설정
    NextCardImageSetting();
}
    private IEnumerator ClockBackGroundAnim(float duration, params int[] cardIndexes)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsedTime / duration);

        // 선택된 카드들만 fillAmount 증가
        foreach (int index in cardIndexes)
        {
            card_clockBack[index].fillAmount = progress;
        }

        yield return null;
    }
}


    public void DrawCard(int fixedCard){
               StartCoroutine(DrawCard_Corutine(fixedCard));
    }

    public IEnumerator DrawCard_Corutine(int fixedCard){
                int cardId = deck[0]; // 덱 맨 위의 카드 ID 가져오기
                int cardStack =  GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == deck[0]).STACK;
                  // int cardLevel = deck[0].STACK;
                deck.RemoveAt(0);    // 덱 맨 위의 카드 제거
                int basicDrawCoolTime = 1;
                float traningCoolTime = GameManager.instance.boardUI.upgradeUI.Traning_DCD_Setting();
                float finalDrawCoolTime = basicDrawCoolTime - traningCoolTime;

                //드로우 딜레이
                magicCards[fixedCard].CardReload();
                yield return StartCoroutine(ClockBackGroundAnim(finalDrawCoolTime,fixedCard));
                Debug.Log("카드 쿨타임 : " + finalDrawCoolTime);

                // 핸드 카드 초기화
                magicCards[fixedCard].CardInit(cardId);
                NextCardImageSetting();
    }

    public void NextCardImageSetting(){
        nextcardImage.sprite = cardDatas[deck[0]].nextcardImage;
    }

    public void CardDescInit(int cardId){
        //카드 터치했을때 카드 설명 UI

        CardDescUi.gameObject.SetActive(true);
        CardDesc_CardName.text = cardDatas[cardId].cardName;//이름 세팅
        CardDesc_CardDesc.text = cardDatas[cardId].cardDesc_Main;//이름 세팅
    }

    //////////////////////
    //덱 관리 UI 관련
    public void ShowPlayerDeck(){

        int selectedDeckNumber = GameManager.instance.dataManager.selectedPresetDeck;
        //현재 플레이어의 활성덱을 보여주는 함수
        DataManager data = GameManager.instance.dataManager;
        float manaAver = 0;
        //오브젝트들 초기화
                for(int i = 0; i < 8; i++){
                        GameManager.instance.boardUI.deckCardUI[i].Init(data.savedDeck[selectedDeckNumber][i]);

                            if(data.savedDeck[selectedDeckNumber][i] != -1){
                                manaAver += cardDatas[data.savedDeck[selectedDeckNumber][i]].cardCost;
                            }
                            else{
                                //빈 카드라면 마나 0
                                manaAver += 0;
                            }
                       
                    }
    
        //텍스트 최산화

        battleDeckText.color = Color.white;
        battleDeckText.text = "전투 덱" + (selectedDeckNumber + 1).ToString() + "   "+ FillteringSavedDeck(selectedDeckNumber).Count + "/8";
        text_manaAverage.text = (manaAver/ FillteringSavedDeck(selectedDeckNumber).Count).ToString("F1");//마나 평균

        if(FillteringSavedDeck(selectedDeckNumber).Count < 4){
            battleDeckText.color = Color.red;
        }
        ShowOwnedCards(selectedDeckNumber);
    }


    public void Click_PresetDeckButton(int touchIndex){

            GameManager.instance.dataManager.selectedPresetDeck = touchIndex;

            for(int index = 0; index < preSet_Deck.Length; index++){
                preSet_Deck[index].mainPanel.color = new Color(0.6f, 0.6f, 0.6f);
            }
            preSet_Deck[touchIndex].mainPanel.color = new Color(1f, 1f, 1f);
            ShowPlayerDeck();

    }

   public List<int> FillteringSavedDeck(int index)
{
    // 현재 savedDeck에서 ID가 -1은 제외하는 함수
    List<int> list = new List<int>(GameManager.instance.dataManager.savedDeck[index]); // savedDeck를 복사
   // Debug.Log(string.Join(", ", GameManager.instance.dataManager.savedDeck[index]) + " 여기에 -1이 있어야됨");

    // savedDeck[index]를 수정하지 않고 list에서만 -1을 제거
    list.RemoveAll(value => value == -1); // -1인값 삭제

   // Debug.Log(string.Join(", ", GameManager.instance.dataManager.savedDeck[index]) + " 여기에 -1이 있어야됨");

    return list;
}



   public void ShowOwnedCards(int index)
{
    // ownedCardset의 자식 오브젝트들을 비활성화
    foreach (Transform child in ownedCardset.transform)
    {
        child.gameObject.SetActive(false);
    }

    // 현재 덱에 포함된 카드 ID 리스트 (ID만 포함)
    List<int> savedCardIds = FillteringSavedDeck(index); 

    // savedDeck에 없는 카드만 필터링
    List<Card> ownedCardList = GameManager.instance.dataManager.havedCardsList
        .Where(card => !savedCardIds.Contains(card.ID)) // savedDeck에 없는 카드만 선택
        .ToList();

    for (int i = 0; i < ownedCardList.Count; i++)
    {
        DeckCard deckCard = GameManager.instance.deckCardPooling.Get(0).GetComponent<DeckCard>();
        deckCard.Init(ownedCardList[i].ID);
        deckCard.inMyDeck = false; // 선택된 DeckCard가 현재 덱에 있는지 여부
    }

     // 📌 스크롤 길이 동적 조절
    int cardCount = ownedCardList.Count;
    float newHeight = 1800 + Mathf.Max(0, (cardCount - 1) / 4) * 400; 
    Scroll_SetRectTransformHeight(newHeight);
}

   public void ShowAllCards()
{
   // AllCardPooling 자식 오브젝트들을 비활성화
    foreach (Transform child in GameManager.instance.AllCardPooling.transform)
    {
        child.gameObject.SetActive(false);
    }

    // 모든 보유 카드 리스트 가져오기
    List<Card> allCardList = GameManager.instance.dataManager.GetALLDeckData();

    // 카드 데이터를 참조할 DeckManager 가져오기
    DeckManager deckManager = GameManager.instance.deckManager;

    // 카드 랭크 순으로 정렬 (normal → rare → epic → legend)
    List<Card> sortedCardList = allCardList
        .OrderBy(card => GetCardRank(card.ID))
        .ToList();

    // 보유한 카드 ID 목록 가져오기
    HashSet<int> ownedCardIDs = new HashSet<int>(GameManager.instance.dataManager.havedCardsList.Select(card => card.ID));

    // 정렬된 카드 리스트로 UI 생성
    for (int i = 0; i < sortedCardList.Count; i++)
    {
        DeckCard deckCard = GameManager.instance.AllCardPooling.Get(0).GetComponent<DeckCard>();

        // ID가 havedCardList에 존재하면 true, 아니면 false
        bool isAcquired = ownedCardIDs.Contains(sortedCardList[i].ID);

        deckCard.Init_ForAllCard(sortedCardList[i],isAcquired);

        deckCard.inMyDeck = false;
        deckCard.inAllCard = true;
    }

    // 📌 스크롤 길이 동적 조절
    int cardCount = sortedCardList.Count;
    float newHeight = 1000 + Mathf.Max(0, (cardCount - 1) / 4) * 400;
    AllCard_Scroll_SetRectTransformHeight(newHeight);
}

// 특정 카드의 Rank를 가져오는 함수
private CardData.CardRank GetCardRank(int cardId)
{
    CardData cardData = cardDatas.FirstOrDefault(data => data.cardId == cardId);
    return cardData != null ? cardData.rank : CardData.CardRank.normal; // 기본값 normal
}


    public void RemoveCard(int cardId){
        //플레이어의 활성덱에서 카드 제거 deck -> owedCardList
        int selectedPresetNumber = GameManager.instance.dataManager.selectedPresetDeck;
        int index = GameManager.instance.dataManager.savedDeck[selectedPresetNumber].IndexOf(cardId);
        GameManager.instance.dataManager.savedDeck[selectedPresetNumber][index] = -1; //-1 is Null


        //UI 최신화
        ShowPlayerDeck();
        TouchedCardSetFalse();//터치카드 비활성화
        
    }
    public void TouchedCardSetFalse(){
    touchedCard[0].SetActive(false);//터치 카드 UI 비활성화
            touchedCard[1].SetActive(false);//터치 카드 UI 비활성화
    }

  public void AddCard(int cardId)
{
    int selectedPresetNumber = GameManager.instance.dataManager.selectedPresetDeck;
    List<int> selectedDeck = GameManager.instance.dataManager.savedDeck[selectedPresetNumber];

    // 덱 상태 확인 로그 추가
    Debug.Log($"Selected Deck: {string.Join(", ", selectedDeck)}");  // 덱 상태 출력

    // -1 값을 가진 첫 번째 인덱스를 찾음
    int index = selectedDeck.IndexOf(-1);

    if (index != -1)
    {
        // -1을 cardId로 변경
        selectedDeck[index] = cardId;
        
        // 덱 UI 업데이트
        ShowPlayerDeck();
        TouchedCardSetFalse();
    }
    else
    {
        Debug.LogWarning("덱이 최대치(8장)에 도달했습니다!");
    }
}



   public void SaveNowDeck()
{   
    int selectedDeckNumber = GameManager.instance.dataManager.selectedPresetDeck;
    //순서 재배치
    GameManager.instance.dataManager.ReorderSavedDeck(selectedDeckNumber);

    // -1이 아닌 카드 개수 확인
    int validCardCount = GameManager.instance.dataManager.savedDeck[selectedDeckNumber].Count(card => card != -1);

    if (validCardCount < 4)
    {
        GameManager.instance.WarningText("덱은 항상 4장 이상이어야 합니다!");
        return;
    }

    GetSavedDeck(GameManager.instance.dataManager.savedDeck[selectedDeckNumber]); // 덱 재구성
   
    HandSetting(); // 덱 재구성 후 다시 카드 뽑기

    GameManager.instance.boardUI.Hide_DeckSettingUI(); // 최종적으로 덱 세팅 UI 종료
}


    /// <summary>
    /// RectTransform의 높이(Height)를 설정합니다.
    /// Anchor 모드가 Top, Stretch 상태에서도 작동합니다.
    /// </summary>
    /// <param name="height">설정할 높이 값</param>
    public void Scroll_SetRectTransformHeight(float height)
    {
        if (cardSettingScroll_Rect == null) return;

        // 현재 RectTransform의 크기 가져오기
        Vector2 sizeDelta = cardSettingScroll_Rect.sizeDelta;
        
        // height만 변경
        sizeDelta.y = height;
        
        // 변경된 크기 적용
        cardSettingScroll_Rect.sizeDelta = sizeDelta;
    }

     public void AllCard_Scroll_SetRectTransformHeight(float height)
    {
        if (cardCollectionScroll_Rect == null) return;

        // 현재 RectTransform의 크기 가져오기
        Vector2 sizeDelta = cardCollectionScroll_Rect.sizeDelta;
        
        // height만 변경
        sizeDelta.y = height;
        
        // 변경된 크기 적용
        cardCollectionScroll_Rect.sizeDelta = sizeDelta;
    }
    

    /// <summary>
    /// RectTransform의 Y 위치를 설정합니다.
    /// Anchor 모드가 Top, Stretch 상태일 때는 anchoredPosition을 사용합니다.
    /// </summary>
    /// <param name="posY">설정할 Y 위치 값</param>
    public void Scroll_SetRectTransformPosY(float posY)
    {
        if (cardSettingScroll_Rect == null) return;

        // 현재 anchoredPosition 가져오기
        Vector2 anchoredPosition = cardSettingScroll_Rect.anchoredPosition;
        
        // Y 위치만 변경
        anchoredPosition.y = posY;
        
        // 변경된 위치 적용
        cardSettingScroll_Rect.anchoredPosition = anchoredPosition;
    }

    /// <summary>
    /// Top, Stretch 상태에서 RectTransform의 위치와 크기를 한 번에 설정합니다.
    /// </summary>
    /// <param name="posY">설정할 Y 위치 값</param>
    /// <param name="height">설정할 높이 값</param>
    public void Scroll_SetRectTransformPositionAndHeight(float posY, float height)
    {
        Scroll_SetRectTransformPosY(posY);
        Scroll_SetRectTransformHeight(height);
    }
    

}





/**************************************************************************************
로그라이크 시스템 삭제
    private void UpdateCandidatePool(List<Card> deck, CardData[] allCards)
{
    // 기존 후보 풀 초기화
    candidatePool.Clear();

    // 1. 업그레이드 가능한 카드 추가
    foreach (var card in deck)
    {
        if (card.STACK < 3) // 레벨 3 미만인 카드만 추가
        {
            //기존 카드보다 1레벨 높은 카드를 풀에 추가.
            candidatePool.Add(new Card(card.ID, card.STACK + 1, 1));
        }
    }
    //1-2 업그레이드
    foreach(var handCard in magicCards){
        if(handCard.cardRank < 3){
            candidatePool.Add(new Card(handCard.cardId, handCard.cardRank + 1, 1));
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
        candidatePool.Add(new Card(cardData.cardId, 1, 1)); // 신규 카드는 레벨 1로 추가
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
        randomCardDescUI.gameObject.SetActive(false);
        
        //선택된 카드 덱에 추가
       // AddCard_ToHand(card);
        GameManager.instance.GamePlay();
    }
/**
public void AddCard_ToHand(Card newCard)
{
    // 1. 덱에서 같은 ID의 카드를 찾음
    var existingCardInDeck = deck.Find(card => card.ID == newCard.ID);

    if (existingCardInDeck != null)
    {
        // 이미 존재하는 카드의 Rank를 올림
        if (existingCardInDeck.STACK < 3)
        {
            existingCardInDeck.STACK += 1;
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
            //existingCardInHand.Init_CardUpgrade(existingCardInHand.cardRank + 1);// 등급 업
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

    public void EndCardUpgrade(){
        GameManager.instance.GamePlayState = true;
        randomCard[0].gameObject.SetActive(false);
         randomCard[1].gameObject.SetActive(false);
          randomCard[2].gameObject.SetActive(false);
          randomCardDescUI.gameObject.SetActive(false);
          GameManager.instance.backG.SetActive(false);//검은 배경 비활성화
    }
    ******************카드 업그레이드 이벤트 삭제
    **/ 

    /** 시스템 삭제
    public void StartUpgradeEvent(){
       // UpgradeEvent(deck, cardDatas);
    }

     public void UpgradeEvent(List<Card> deck, CardData[] allCards){
         // 1. 후보 카드 풀 초기화
        //UpdateCandidatePool(deck, allCards);

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
           // int index = 0;//첫번째 randomCard
         //   RandomCardSelectedSetting(index);


        
     }
     시스템 삭제//
     **/ 
    