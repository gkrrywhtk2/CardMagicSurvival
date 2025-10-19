using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardImage : MonoBehaviour
{
    [Header("Object")]
    public Image cardFrame;
    public Image cardDeco;
    public Image mainImage;
    public Image manaCostImage;
    public TMP_Text manaCost_text;
    private MagicCard card;
    void Awake()
    {
        card = transform.GetComponentInParent<MagicCard>();
    }

    public void Init(int id)
    {
        FrameInit();
        mainImageInit();
        CostInit();
    }
    public void FrameInit()
    {
        RankType rank = (RankType)card.cardData.rank; // 이미 enum이면 그대로 사용
        Color rankColor = GameManager.instance.dataManager.colorDatas.GetColor(rank);
        cardFrame.color = rankColor;
        cardDeco.color = rankColor;
    }
    public void mainImageInit()
    {
        mainImage.sprite = card.cardData.cardImage;
    }
    public void CostInit()
    {
        int cost = card.cardData.cardCost;
        manaCost_text.text = cost.ToString();
    }

    public void CardAlpha0_Range(bool shouldRangeBeActive){
    //카드 드래그시 카드가 범위 카드라면 카드를 투명화
    if(shouldRangeBeActive == true){
        Color cardColor = cardFrame.color;
        Color manaColor = manaCostImage.color;
        Color textColor = manaCost_text.color;
        Color decoColor = cardDeco.color;

        cardColor.a = 0;
        manaColor.a = 0;
        textColor.a = 0;
            decoColor.a = 0;
        
        cardFrame.color = cardColor;
        manaCostImage.color = manaColor;
        manaCost_text.color = textColor;
        cardDeco.color = decoColor;
    }else{
        CardAlpha1_Range();
    }
}
public void CardAlpha1_Range(){

    Color cardColor = cardFrame.color;
    Color manaColor = manaCostImage.color;
    Color textColor = manaCost_text.color;
    Color decoColor = cardDeco.color;

    cardColor.a = 1;
    manaColor.a = 1;
    textColor.a = 1;
    decoColor.a = 1;

    cardFrame.color = cardColor;
    manaCostImage.color = manaColor;
    manaCost_text.color = textColor;
    cardDeco.color = decoColor;
    }
    
}
