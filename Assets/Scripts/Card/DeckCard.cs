using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeckCard : MonoBehaviour
{
    public Card deckCard;//{cardID, cardLevel}
    public Image cardSprite;//카드 메인 이미지
    public Image manaSprite;//마나 이미지
    public TMP_Text costText;//카드 코스트 텍스트
    public GameObject gaze;//fill의 부모 오브젝트
    public Image Fill;//0/5 fillgaze
    public TMP_Text fillText;//1/5 text
    public GameObject stackBackGround;//중첩 텍스트의 부모 오브젝트
    public TMP_Text stackText;//중첩 텍스트

    public void Init(Card card){

        if(card.ID == -1){
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
        //데이터 처리
        deckCard = card;
       // deckCard.ID = card.ID; 이렇게 초기화 하면 안됨, 오답노트로 남겨두자 
       // deckCard.STACK = card.STACK;
       // deckCard.COUNT = card.COUNT;
        cardSprite.sprite = deckData.cardDatas[deckCard.ID].cardImage;
        costText.text = deckData.cardDatas[deckCard.ID].cardCost.ToString();
        Fill.fillAmount = Mathf.Clamp01((float)deckCard.COUNT / 5);
        fillText.text =  deckCard.COUNT.ToString() + "/ 5";
        stackText.text = "중첩 " + deckCard.STACK.ToString();
        }

    }
    public void NullCardInit(){
        //존재하지 않는 카드 -1 일경우 흑백 처리 
        cardSprite.gameObject.SetActive(false);
        manaSprite.gameObject.SetActive(false);
        gaze.gameObject.SetActive(false);
        stackBackGround.gameObject.SetActive(false);

    }


}
