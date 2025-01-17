using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class RandomCard : MonoBehaviour, IEndDragHandler, IBeginDragHandler
{
    public Card nowCard;
    public int cardId;
    public int cardRank;
   public Image cardImage;
    public Image[] stars;// 카드 등급 이미지
    public Sprite star_True;
    public Sprite star_False;
    public TMP_Text costText;//코스트 숫자

    public void Init(Card card){
        nowCard = card;
        this.cardId = nowCard.ID;
        this.cardRank = nowCard.Rank;
        CardData data =  GameManager.instance.deckManager.cardDatas[card.ID];
        cardImage.sprite = data.cardImage;
        costText.text = data.cardCost.ToString();
        RankImageSetting(cardRank);
    }
     public void RankImageSetting(int rank){
    //카드 등급 이미지(별) 세팅

    //초기화
    stars[0].sprite = star_False;
    stars[1].sprite = star_False;
    stars[2].sprite = star_False;

        switch(rank){
            case 1:
                stars[0].sprite = star_True;
            break;

            case 2:
              stars[0].sprite = star_True;
              stars[1].sprite = star_True;
            break;

            case 3:
               stars[0].sprite = star_True;
               stars[1].sprite = star_True;
               stars[2].sprite = star_True;
            break;

            default:
            break;
        }
    }

    public void SendCardInfo(){
        GameManager.instance.deckManager.TakeCardInfo(nowCard);
        //GameManager.instance. nextWaveButton 카드 선택시 다음 웨이브로 넘어가야할 차례
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
