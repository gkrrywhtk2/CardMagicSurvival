using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.RankSystem;
using Game.InGameCardManager;
using Game.CardData;

public class CardImage : MonoBehaviour
{
    [Header("Object")]
    public Image cardFrame;
    public Image cardDeco;
    public Image mainImage;
    public Image manaCostImage;
    public TMP_Text manaCost_text;
    
    public int cardId { get; private set; }
  
    // ✅ PlayerCard를 직접 받는 Init (권장)
    public void Init(PlayerCard playerCard)
    {
        cardId = playerCard.ID;
        RankType rank = CardData.Instance.cardScritableData[cardId].rank;
        FrameInit(rank); // 등급 직접 전달
        MainImageInit();
        CostInit();
    }
    
    // ✅ int만 받는 Init (호환성 유지)
    public void Init(int id)
    {
        cardId = id;
        FrameInit(CardData.Instance.cardScritableData[cardId].rank);
        MainImageInit();
        CostInit();
    }
    
    // ✅ 등급을 직접 받는 FrameInit
    public void FrameInit(RankType rank)
    {
        UpdateFrameColor(rank);
    }
    
    public void UpdateFrameColor(RankType rank)
    {
        Color rankColor = RankDatas.GetColor(rank);
        cardFrame.color = rankColor;
        cardDeco.color = rankColor;
    }
    
    public void MainImageInit()
    {
        mainImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[cardId].cardImage;
    }
    
    public void CostInit()
    {
        int cost = LocalDataManager.Instance.cardData.cardScritableData[cardId].cardCost;
        manaCost_text.text = cost.ToString();
    }

    public void CardAlpha0_Range(bool shouldRangeBeActive)
    {
        if (shouldRangeBeActive)
        {
            SetAlpha(0f);
        }
        else
        {
            CardAlpha1_Range();
        }
    }
    
    public void CardAlpha1_Range()
    {
        SetAlpha(1f);
    }
    
    private void SetAlpha(float alpha)
    {
        Color main = mainImage.color;
        Color cardColor = cardFrame.color;
        Color manaColor = manaCostImage.color;
        Color textColor = manaCost_text.color;
        Color decoColor = cardDeco.color;

        main.a = alpha;
        cardColor.a = alpha;
        manaColor.a = alpha;
        textColor.a = alpha;
        decoColor.a = alpha;

        mainImage.color = main;
        cardFrame.color = cardColor;
        manaCostImage.color = manaColor;
        manaCost_text.color = textColor;
        cardDeco.color = decoColor;
    }
}