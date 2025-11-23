using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Game.RankSystem;

public class CardImage : MonoBehaviour
{
    [Header("Object")]
    public Image cardFrame;
    public Image cardDeco;
    public Image mainImage;
    public Image manaCostImage;
    public TMP_Text manaCost_text;
    
    private int currentCardId;
  
    public void Init(int id)
    {
        currentCardId = id;
        FrameInit();
        MainImageInit();
        CostInit();
    }
    
    public void FrameInit()
    {
        // ✅ AccountCardManager에서 카드 정보 가져오기
        PlayerCard card = AccountCardManager.Instance.GetCardById(currentCardId);
        
        if (card == null)
        {
            Debug.LogError($"[CardImage] 카드 ID {currentCardId}를 찾을 수 없습니다!");
            return;
        }
        
        RankType rank = card.currentRarity;
        Color rankColor = RankDatas.GetColor(rank);
        cardFrame.color = rankColor;
        cardDeco.color = rankColor;
    }
    
    public void MainImageInit()
    {
        // ✅ LocalDataManager를 통해 카드 이미지 가져오기
        mainImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[currentCardId].cardImage;
    }
    
    public void CostInit()
    {
        // ✅ LocalDataManager를 통해 카드 코스트 가져오기
        int cost = LocalDataManager.Instance.cardData.cardScritableData[currentCardId].cardCost;
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
    
    // ✅ 리팩토링: 중복 코드 제거
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