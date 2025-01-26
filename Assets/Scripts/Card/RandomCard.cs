using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RandomCard : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public int RandomCardNum;//012
    public Card nowCard;
    public int cardId;
    public int cardLevel;
   public Image cardImage;
   public Image manaSprite;
    public Image[] stars;// 카드 등급 이미지
    public Sprite star_True;
    public Sprite star_False;
    public TMP_Text costText;//코스트 숫자
    public GameObject outLine;//선택 되었을때 아웃라인
    public Image selectFill;//시계 방향 Fill 이미지
    private float Fill_Max = 2;//2초 지속되면 카드 선택
    private float Fill_now = 0;//카드 누르고 있는 시간
    private bool thisTouch = false;
    public Animator create;

    private void Update(){
        FillUp();
        CardSelect();

    }
    public void FillUp(){
        if(thisTouch == false)
            return;

            Fill_now += Time.unscaledDeltaTime;
    }
        
    
    public void CardTouchDown(){

        if(SingleEventTrigger.single.oneTouch == true)
            return;

            SingleEventTrigger.single.oneTouch = true;
            thisTouch = true;
    }
    public void CardTouchUp(){
        SingleEventTrigger.single.oneTouch = false;
        thisTouch = false;
        Fill_now = 0;
        selectFill.fillAmount = Fill_now/Fill_Max;
        

    }
    public void CardSelect(){
        float value = Fill_now/Fill_Max;
        if(value < 0.1f)
            return;

        selectFill.fillAmount = value;
        if(Fill_now >= Fill_Max){
            //선택한 카드 추가
            GameManager.instance.deckManager.TakeCardInfo(new Card(cardId,cardLevel));
            SingleEventTrigger.single.oneTouch = false;
            thisTouch = false;
            return;
        }
    }

    public void Init(Card card){
        selectFill.fillAmount = 0;
        outLine.gameObject.SetActive(false);
        create.gameObject.SetActive(true);
        SingleEventTrigger.single.oneTouch = false;
        thisTouch = false;
        Fill_now = 0;
        nowCard = card;
        this.cardId = nowCard.ID;
        this.cardLevel = nowCard.Rank;
        CardData data =  GameManager.instance.deckManager.cardDatas[cardId];
        cardImage.gameObject.SetActive(false);
        stars[0].gameObject.SetActive(false);
        stars[1].gameObject.SetActive(false);
        stars[2].gameObject.SetActive(false);
        manaSprite.gameObject.SetActive(false);
        costText.gameObject.SetActive(false);

    }
    public void ImageSetting(){
        CardData data =  GameManager.instance.deckManager.cardDatas[cardId];
        cardImage.gameObject.SetActive(true);
        cardImage.sprite = data.cardImage;
        manaSprite.gameObject.SetActive(true);
        costText.gameObject.SetActive(true);
        costText.text = data.cardCost.ToString();
        RankImageSetting(cardLevel);
        create.gameObject.SetActive(false);
    }
   
    public void RankImageSetting(int level){
    //카드 레벨 이미지(별) 세팅
        stars[0].gameObject.SetActive(true);
        stars[1].gameObject.SetActive(true);
        stars[2].gameObject.SetActive(true);
    //초기화
    stars[0].sprite = star_False;
    stars[1].sprite = star_False;
    stars[2].sprite = star_False;
    stars[0].GetComponent<Animator>().SetBool("Blank",false);
    stars[1].GetComponent<Animator>().SetBool("Blank",false);
    stars[2].GetComponent<Animator>().SetBool("Blank",false);
        switch(level){
            case 1:
                stars[0].sprite = star_True;
                stars[0].GetComponent<Animator>().SetBool("Blank",true);//별 깜빡
            break;

            case 2:
            stars[0].sprite = star_True;
            stars[1].sprite = star_True;
                stars[1].GetComponent<Animator>().SetBool("Blank",true);//별 깜빡
            break;

            case 3:
               stars[0].sprite = star_True;
               stars[1].sprite = star_True;
               stars[2].sprite = star_True;
                 stars[2].GetComponent<Animator>().SetBool("Blank",true);//별 깜빡
            break;

            default:
            break;
        }
    }

    public void SendCardInfo(){
         //GameManager.instance.deckManager.TakeCardInfo(nowCard); 일단 대기
        //GameManager.instance. nextWaveButton 카드 선택시 다음 웨이브로 넘어가야할 차례
      GameManager.instance.deckManager.RandomCardSelectedSetting(RandomCardNum);
        
    }

     public void OnEndDrag(PointerEventData eventData){
        SendCardInfo();
        Debug.Log($"OnBeginDrag called for {gameObject.name}");
     }
     public void OnBeginDrag(PointerEventData eventData){
 Debug.Log($"OnEndDrag called for {gameObject.name}");

     }
      public void OnButtonClick()
    {
        Debug.Log("Button clicked!");
    }
    

}
