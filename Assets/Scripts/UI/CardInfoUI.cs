using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using RANK;
using System.Linq;
using Unity.Mathematics; // LINQ 사용

public class CardInfoUI : MonoBehaviour
{
    Card thisCard;//카드 아이디, 중첩 계수, 보유 계수
    CardData cardData;//프론트엔드 카드 데이터
    public TMP_Text title;//타이틀, 한영 변환용
    public TMP_Text cardRank_Text;//카드 등급
    public TMP_Text cardName_Text;//카드 이름
    public DeckCard deckCard;//카드 이미지 세팅
    public TMP_Text cardEffect_Text;//한영 변환
    public TMP_Text cardEffect_VarText;//카드 효과
    public TMP_Text cardDesc_Text;//한영 변환
    public TMP_Text[] cardDescNameLine_Text;//카드 정보 0번쨰 줄 
    public TMP_Text[] cardDescNameLine_VarText;//카드 정보 0번째줄 효과
    public GameObject touchedCard;//deckCardTouch했을때 정보, 제거, 사용 뜨는 그 오브젝트
    //
    public GameObject warningText;
    public GameObject maxWariningText;

    

       //TextColor
    private Color commonColor_W, commonColor;
    private Color rareColor_W, rareColor;
    private Color epicColor_W, epicColor;
    private Color legendColor_W, legendColor;

    private void Awake()
    {
    // 밝은 색상 (획득한 카드)
    commonColor_W = new Color(0.7f, 0.7f, 0.7f, 1f);
    rareColor_W = new Color(0.3f, 0.5f, 0.9f, 1f);
    epicColor_W = new Color(0.6f, 0.3f, 0.8f, 1f);
    legendColor_W = new Color(1f, 0.7f, 0.2f, 1f);

    // 어두운 색상 (미획득 카드)
    commonColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    rareColor = new Color(0.2f, 0.3f, 0.6f, 1f);
    epicColor = new Color(0.4f, 0.2f, 0.5f, 1f);
    legendColor = new Color(0.8f, 0.5f, 0.1f, 1f);
    }
    public void Init(Card card){
    //데이터 세팅
        thisCard = card;
        cardData = GameManager.instance.deckManager.cardDatas[card.ID];

    //카드 랭크Text 세팅
    cardRank_Text.text = cardData.rank switch
    {
    CardData.CardRank.normal => "일반",
    CardData.CardRank.rare => "희귀",
    CardData.CardRank.epic  => "영웅",
    CardData.CardRank.legend => "전설",
    _ => "알 수 없음"
    };
    cardRank_Text.color = cardData.rank switch
    {
    CardData.CardRank.normal => commonColor_W,
    CardData.CardRank.rare => rareColor_W,
    CardData.CardRank.epic  => epicColor_W,
    CardData.CardRank.legend => legendColor_W,
    _ => commonColor_W
    };


    //카드 이름Text 세팅
    cardName_Text.text = cardData.cardName;

    //카드 이미지 세팅
    deckCard.Init(thisCard.ID);

    //카드 효과 설명 세팅
    cardEffect_VarText.text = cardData.cardDesc_Main;

    //카드 정보 세팅
    int LineCount = cardData.infoTags.Length;//카드 정보에 표시할 줄 계수
    for(int i = 0; i < cardDescNameLine_Text.Length; i ++){
        cardDescNameLine_Text[i].text = "";//초기화
        cardDescNameLine_VarText[i].text = "";//초기화
    }
   
    for(int i = 0; i < LineCount; i++){
        cardDescNameLine_Text[i].text = cardData.infoTags[i] switch{
           CardData.InfoTag.damage => "데미지",
            CardData.InfoTag.count => "공격 횟수",
             CardData.InfoTag.duration => "지속 시간",
              CardData.InfoTag.range => "범위",
               CardData.InfoTag.manarecovery => "초당 마나 회복량",
                 CardData.InfoTag.speedUp => "추가 이동속도",
                 CardData.InfoTag.Heal => "체력 회복",
           _=> "알 수 없음"
        };
    }

    for(int i = 0; i < LineCount; i++){
        cardDescNameLine_VarText[i].text = cardData.infoTags[i] switch{

           CardData.InfoTag.damage => 
                cardData.GetDamage(thisCard.STACK).ToString()+ 
                "%" + "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetDamage(thisCard.STACK + 1).ToString()+"</color>" +
                "<color=#00FF00>%</color>",

            CardData.InfoTag.count =>  cardData.GetCount(thisCard.STACK).ToString()+ 
                "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetCount(thisCard.STACK + 1).ToString()+"</color>",
                
            CardData.InfoTag.duration => cardData.GetDuration(thisCard.STACK).ToString()+ 
                "s" + "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetDuration(thisCard.STACK + 1).ToString()+"</color>" +
                "<color=#00FF00>s</color>",

            CardData.InfoTag.range => cardData.GetRangeForUser(thisCard.STACK).ToString()+ 
                "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetRangeForUser(thisCard.STACK + 1).ToString()+"</color>",

            CardData.InfoTag.manarecovery =>   cardData.GetManaRecovery(thisCard.STACK).ToString()+ 
                "%" + "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetManaRecovery(thisCard.STACK + 1).ToString()+"</color>" +
                "<color=#00FF00>%</color>",

            CardData.InfoTag.speedUp =>   cardData.GetSpeedUp(thisCard.STACK).ToString()+ 
                  "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetSpeedUp(thisCard.STACK + 1).ToString()+"</color>",

                CardData.InfoTag.Heal =>   cardData.GetHeal(thisCard.STACK).ToString()+ 
                "%" + "<color=#00FF00>-> </color>" +  "<color=#00FF00>" +
                cardData.GetHeal(thisCard.STACK + 1).ToString()+"</color>" +
                "<color=#00FF00>%</color>",
           _=> "알 수 없음"
        };
    }
}

public void OffButton(){
    GameManager.instance.deckManager.touchedCard.gameObject.SetActive(false); //TouchedCard 비활성화
    GameManager.instance.boardUI.cardInfoUI.gameObject.SetActive(false);
 
}
public void OnButton(){
    GameManager.instance.boardUI.cardInfoUI.gameObject.SetActive(true);
}
    public void StackUpgradeButton(){
         Card targetCard = GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == thisCard.ID);

        if(thisCard.STACK >= 10){
            maxWariningText.gameObject.SetActive(true);
            return;
        }

        if(thisCard.COUNT >= ReturnRequireCount(targetCard.STACK)){//레벨에 따른 필요 재료 수
        // ID가 1인 카드 찾기
        targetCard.COUNT -= 5;//카드 5개 소진
        targetCard.STACK += 1;//카드 업그레이드
        Init(targetCard);
        GameManager.instance.deckManager.ShowPlayerDeck();
        DeckCard infoCard = GameManager.instance.boardUI.deckCardButtons.GetComponent<DeckCard>();
        infoCard.gameObject.SetActive(true);
        infoCard.Init(targetCard.ID);

        GameManager.instance.deckManager.magicCards[0].CardInitWhenStackUpgrade();
            GameManager.instance.deckManager.magicCards[1].CardInitWhenStackUpgrade();
                GameManager.instance.deckManager.magicCards[2].CardInitWhenStackUpgrade();
                //Debug.Log("StackUpgradeButton");
        }else{
            warningText.gameObject.SetActive(true);
        }
    }
    public int ReturnRequireCount(int nowStack){
        //업그레이드 시 필요한 재료 수를 리턴하는 함수
        int index = 0;
        index = 2 + nowStack ;
        return index;
    }
}
