using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // LINQ 사용
using UnityEngine.EventSystems;

public class DeckCard : MonoBehaviour , IPointerDownHandler, IPointerUpHandler
{
    public Card deckCard;
    public Image cardSprite;//카드 메인 이미지
    public Image manaSprite;//마나 이미지
    public TMP_Text costText;//카드 코스트 텍스트
    public GameObject gaze;//fill의 부모 오브젝트
    public Image Fill;//0/5 fillgaze
    public TMP_Text fillText;//1/5 text
    public GameObject stackBackGround;//중첩 텍스트의 부모 오브젝트
    public TMP_Text stackText;//중첩 텍스트
    public bool inMyDeck;//현재 나의 카드목록에 올라와있는 덱 카드인가?
    public bool isTouchInfo;//이게 활성화 되어있으면 deckCard 오브젝트가 아닌, 터치시 연출되는 상세 정보 보기 버튼 나오는 toucheddeckCard임
    public bool inAllCard;//모든 카드 보기 안에 있는 DeckCard일 경우 true
    RectTransform rect;
    public GameObject upArrow;//업그레이드 가능 오브젝트
    public GameObject lockImage;
    public bool isAcquired;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }
  void OnEnable()
  {
    isAcquired = true;
  }

  public void Init(int cardid){
        deckCard =  GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == cardid);
        if(deckCard == null){
            deckCard = new Card(-1, 0 , 0);
        }
        //deckCard = card; //카드 데이터 처리
        if(deckCard.ID == -1){
        NullCardInit();
        }
        else{
            //정상 처리
        DeckManager deckData = GameManager.instance.deckManager;
        DataManager data = GameManager.instance.dataManager;
        cardSprite.gameObject.SetActive(true);
        manaSprite.gameObject.SetActive(true);
        gaze.gameObject.SetActive(true);
        stackBackGround.gameObject.SetActive(true);
       
       
       // deckCard.ID = card.ID; 이렇게 초기화 하면 안됨, 오답노트로 남겨두자 
       // deckCard.STACK = card.STACK;
       // deckCard.COUNT = card.COUNT;
       int requireCount = ReturnRequireCount(deckCard.STACK);//업그레이드에 필요한 재료 수
        cardSprite.sprite = deckData.cardDatas[deckCard.ID].cardImage;
        costText.text = deckData.cardDatas[deckCard.ID].cardCost.ToString();
        Fill.fillAmount = Mathf.Clamp01((float)deckCard.COUNT / requireCount);
        fillText.text =  deckCard.COUNT.ToString() + "/ " + requireCount;
        stackText.text = "Lv. " + deckCard.STACK.ToString();
        upArrow.gameObject.SetActive(Fill.fillAmount >= 1);

        if(deckCard.STACK >= 10){
            //카드 만랩은 10
            Fill.fillAmount = 1;
            fillText.text = "MAX";
            upArrow.gameObject.SetActive(false);
        }
        }
        inAllCard = false;

    }

    public void Init_ForAllCard(Card card, bool isAcquired){
         //정상 처리
         this.isAcquired = isAcquired;
        deckCard = card;
        DeckManager deckData = GameManager.instance.deckManager;
        cardSprite.gameObject.SetActive(true);
        manaSprite.gameObject.SetActive(true);
        gaze.gameObject.SetActive(true);
        stackBackGround.gameObject.SetActive(true);
        lockImage.gameObject.SetActive(!isAcquired);
         manaSprite.color = new Color(1,1,1);
         cardSprite.color = new Color(1,1,1);

       // deckCard.ID = card.ID; 이렇게 초기화 하면 안됨, 오답노트로 남겨두자 
       // deckCard.STACK = card.STACK;
       // deckCard.COUNT = card.COUNT;
       int requireCount = ReturnRequireCount(card.STACK);//업그레이드에 필요한 재료 수
        cardSprite.sprite = deckData.cardDatas[card.ID].cardImage;
        costText.text = deckData.cardDatas[card.ID].cardCost.ToString();
        Fill.fillAmount = Mathf.Clamp01((float)card.COUNT / requireCount);
        fillText.text =  card.COUNT.ToString() + "/ " + requireCount;
        stackText.text = "Lv. " + card.STACK.ToString();
        upArrow.gameObject.SetActive(Fill.fillAmount >= 1);
            if(card.STACK >= 10){
                //카드 만랩은 10
                Fill.fillAmount = 1;
                fillText.text = "MAX";
                upArrow.gameObject.SetActive(false);
            }

        if(isAcquired != true){
              manaSprite.color = new Color(0.8f, 0.8f, 0.8f);
            cardSprite.color =  new Color(0.8f, 0.8f, 0.8f);
        }
        inAllCard = true;
          

    }
     public int ReturnRequireCount(int nowStack){
        //업그레이드 시 필요한 재료 수를 리턴하는 함수
        int index = 0;
        index = 2 + nowStack ;
        return index;
    }
    public void NullCardInit(){
        //존재하지 않는 카드 -1 일경우 흑백 처리 
        cardSprite.gameObject.SetActive(false);
        manaSprite.gameObject.SetActive(false);
        gaze.gameObject.SetActive(false);
        stackBackGround.gameObject.SetActive(false);

    }
    public void CardTouch()
    {
        if (deckCard == null || deckCard.ID == -1) {
        Debug.LogWarning("deckCard is null or invalid.");
        return;
    }
       // Debug.Log(deckCard.ID);
        if(isTouchInfo == true)
            return;
        if(deckCard.ID == -1)
            return;
        Debug.Log("카드 터치 : cardID: " + deckCard.ID);

       //// if(isAcquired != true){
            //CardInfoUIOn();
           // return;
      //  }

        if(inAllCard == true){
           // DeckCard touchedCard = GameManager.instance.deckManager.touchedCard[1].GetComponent<DeckCard>();
           // touchedCard.gameObject.SetActive(true);
           // touchedCard.Init(deckCard.ID);
           CardInfoUIOn();
        }else{
            DeckCard infoCard = GameManager.instance.boardUI.deckCardButtons.GetComponent<DeckCard>();
            infoCard.gameObject.SetActive(true);
            infoCard.Init(deckCard.ID);
        }
    

        // 카드를 터치했을 때, 해당 카드의 위치를 가져옵니다.
        Vector3 cardPosition = cardSprite.transform.position;

        // 버튼을 카드 위치로 이동시키기
        int cardID = deckCard.ID;
        MoveButtonToPosition(cardPosition, cardID, inAllCard);
        //gameObject.SetActive(false);
     
    }

    // 버튼을 카드 위치로 이동시키는 함수
    private void MoveButtonToPosition(Vector3 targetPosition, int card , bool inallCard)
    {
        targetPosition.y += 175;
        // 카드의 월드 좌표를 버튼의 월드 좌표로 설정
        if(inallCard == true){
            DeckCard touchedCard = GameManager.instance.deckManager.touchedCard[1].GetComponent<DeckCard>();
            touchedCard.transform.position = targetPosition;
        }else{
             DeckCard cardTouched = GameManager.instance.boardUI.deckCardButtons.GetComponent<DeckCard>();
        cardTouched.transform.position = targetPosition;

                //모든 카드 보기UI 상에 위치한 터치 카드라면 , 사용,제거 버튼을 사용하지 않음
                if(cardTouched.GetComponent<TouchedCard>().isCollection == true)
                return;


        if(inMyDeck == true){
            //활성 카드라면 제거 버튼 활성화
            cardTouched.GetComponent<TouchedCard>().removeButton.gameObject.SetActive(true);
            cardTouched.GetComponent<TouchedCard>().useButton.gameObject.SetActive(false);
        }else{
            //반대 경우
            cardTouched.GetComponent<TouchedCard>().removeButton.gameObject.SetActive(false);
            cardTouched.GetComponent<TouchedCard>().useButton.gameObject.SetActive(true);
        }
        }
        
    }
    public void CardInfoUIOn(){
       CardInfoUI cardinfo =  GameManager.instance.boardUI.cardInfoUI;
       cardinfo.gameObject.SetActive(true);
       cardinfo.Init(deckCard, isAcquired);
      
       GameManager.instance.deckManager.TouchedCardSetFalse();
      
    }
    public void Button_RemoveCard(){
        GameManager.instance.deckManager.RemoveCard(deckCard.ID);
      
    }
    public void Button_ADDCard(){
        GameManager.instance.deckManager.AddCard(deckCard.ID);
       
    }
   public void OnPointerDown(PointerEventData eventData)
    {
        if (!isTouchInfo)
            rect.localScale = new Vector2(2.2f, 2.95f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isTouchInfo)
            rect.localScale = new Vector2(2.3f, 3f);
    }
    



}
