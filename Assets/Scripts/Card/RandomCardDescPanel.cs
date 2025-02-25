using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class RandomCardDescPanel : MonoBehaviour
{
    public TMP_Text playerLvText;
    public TMP_Text nameText;
    public TMP_Text descText;
    public TMP_Text rankText; 
    public TMP_Text newText;
    public TMP_Text cardLvText;

     private Color[] colors = new Color[]
    {
        new Color(179f / 255f, 179f / 255f, 179f / 255f), // 노말 (회색)
        new Color(0f / 255f, 123f / 255f, 255f / 255f),    // 희귀 (파랑)
        new Color(147f / 255f, 0f / 255f, 255f / 255f),    // 영웅 (보라)
        new Color(255f / 255f, 215f / 255f, 0f / 255f)     // 전설 (노랑)
    };

    private Outline outline;

    private void Awake() {
        outline = GetComponent<Outline>();
    }
    public void UISetting(CardData data, int level){
        playerLvText.text = "현재 레벨 :" + GameManager.instance.player.playerStatus.playLevel.ToString();

        nameText.text = data.cardName;
        newText.gameObject.SetActive(false);
        int CardRank = (int)data.rank;//카드 등급
        outline.effectColor = colors[CardRank];//panel 아웃라인 색상

        //랭크 text 세팅
        switch(data.rank){
            case  CardData.CardRank.normal :
            rankText.text  = "일반";
            break;
            case CardData.CardRank.rare :
            rankText.text = "희귀";
            break;
            case CardData.CardRank.epic :
            rankText.text = "영웅";
            break;
            case CardData.CardRank.legend :
            rankText.text = "전설";
            break;
        }
        rankText.color = colors[CardRank];
        //랭크 text 세팅 종료


        switch(level){
            case 1:
           // descText.text = data.cardDescLv1;
            cardLvText.text = "Lv1";
            newText.gameObject.SetActive(true);
            break;
            case 2:
            cardLvText.text = "Lv2";
          //  descText.text = data.cardDescLv2;
            break;
            case 3:
          //  descText.text = data.cardDescLv3;
            cardLvText.text = "Lv3";
            break;
        }
        
    }
}
