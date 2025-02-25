using RANK;
using TMPro;
using UnityEngine;

public class CardInfoUI : MonoBehaviour
{
    Card thisCard;//카드 아이디, 중첩 계수, 보유 계수
    CardData cardData;//프론트엔드 카드 데이터
    public TMP_Text title;//타이틀, 한영 변환용
    public TMP_Text cardRank_Text;//카드 등급
    public TMP_Text cardName_Text;//카드 이름
    public DeckCard cardImgae;//카드 이미지 세팅
    public TMP_Text cardEffect_Text;//한영 변환
    public TMP_Text cardEffect_VarText;//카드 효과
    public TMP_Text cardDesc_Text;//한영 변환
    public TMP_Text[] cardDescNameLine_Text;//카드 정보 0번쨰 줄 
    public TMP_Text[] cardDescNameLine_VarText;//카드 정보 0번째줄 효과
    public TMP_Text cardDescNameLine1_Text;
    public TMP_Text cardDescNameLine1_VarText;
    public TMP_Text cardDescNameLine2_Text;
    public TMP_Text cardDescNameLine2_VarText;

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


    //카드 이름Text 세팅
    cardName_Text.text = cardData.cardName;

    //카드 이미지 세팅
    cardImgae.Init(thisCard);

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
               CardData.InfoTag.manarecovery => "마나 회복량",
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
           _=> "알 수 없음"
        };
    }
}

public void OffButton(){
    GameManager.instance.boardUI.cardInfoUI.gameObject.SetActive(false);
}
public void OnButton(){
    GameManager.instance.boardUI.cardInfoUI.gameObject.SetActive(true);
}
}
