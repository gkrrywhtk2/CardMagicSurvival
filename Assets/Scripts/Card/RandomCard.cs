using TMPro;
using Unity.VisualScripting;
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
    public Image[] stars;// 카드 등급 이미지
    public Sprite star_True;
    public Sprite star_False;
    public TMP_Text costText;//코스트 숫자
    public GameObject outLine;//선택 되었을때 아웃라인
    public Image selectFill;//시계 방향 Fill 이미지
    private float Fill_Max = 2;//2초 지속되면 카드 선택
    private float Fill_now = 0;//카드 누르고 있는 시간
    private bool touchOn = false;

    private void Update(){
        FillUp();
        CardSelect();

    }
    public void FillUp(){
        if(touchOn == true)
            Fill_now += Time.deltaTime;
        }
    
    public void CardTouchDown(){
        if(Input.touchCount > 1)
            return;

        touchOn = true;
    }
    public void CardTouchUp(){
         touchOn = false;
         Fill_now = 0;

    }
    public void CardSelect(){
        selectFill.fillAmount = Fill_now/Fill_Max;
        if(Fill_now >= Fill_Max){
            //선택한 카드 추가
            GameManager.instance.deckManager.TakeCardInfo(new Card(cardId,cardLevel));
            //GameManager.instance.deckManager.EndCardUpgrade();
            return;
        }
    }

    public void Init(Card card){
        touchOn = false;
        Fill_now = 0;
        nowCard = card;
        this.cardId = nowCard.ID;
        this.cardLevel = nowCard.Rank;
        CardData data =  GameManager.instance.deckManager.cardDatas[card.ID];
        cardImage.sprite = data.cardImage;
        costText.text = data.cardCost.ToString();
        RankImageSetting(cardLevel);
    }
     public void RankImageSetting(int level){
    //카드 레벨 이미지(별) 세팅

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
