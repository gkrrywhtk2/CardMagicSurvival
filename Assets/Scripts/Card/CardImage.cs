using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.RankSystem;
using Game.InGameCardManager;

public class CardImage : MonoBehaviour
{
    [Header("Object")]
    public Image cardFrame;
    public Image cardDeco;
    public Image mainImage;
    public Image manaCostImage;
    public TMP_Text manaCost_text;
    
    public int cardId { get; private set; }
  
    public void Init(int id)
    {
        cardId = id;
        FrameInit();
        MainImageInit();
        CostInit();
    }
    
    public void FrameInit()
    {
        RankType rank;
        
        // ✅ GameManager를 통해 InGameCardManager 접근
        if (GameManager.instance != null && 
            GameManager.instance.inGameCardManager != null && 
            GameManager.instance.inGameCardManager.deckManage != null)
        {
            PlayerCard inGameCard = GameManager.instance.inGameCardManager.deckManage.Find(c => c.cardId == cardId);
            
            if (inGameCard != null)
            {
                // ✅ 인게임 덱에 있으면 인게임 등급 사용
                rank = inGameCard.currentRarity;
                Debug.Log($"[CardImage] 카드 {cardId} - InGame 등급: {rank}");
            }
            else
            {
                // ✅ 인게임 덱에 없으면 계정 등급 사용
                PlayerCard accountCard = AccountCardManager.Instance.GetCardById(cardId);
                rank = accountCard != null ? accountCard.currentRarity : RankType.Uncommon;
                Debug.Log($"[CardImage] 카드 {cardId} - Account 등급: {rank}");
            }
        }
        else
        {
            // ✅ GameManager가 없거나 InGameCardManager가 없으면 계정 등급 사용
            PlayerCard accountCard = AccountCardManager.Instance.GetCardById(cardId);
            rank = accountCard != null ? accountCard.currentRarity : RankType.Uncommon;
        }
        
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