using System;
using UnityEngine;
using UnityEngine.UI;

public class SpellCard_Deck : MonoBehaviour
{
    public int slotId;
    public CardImage cardImage;
    public Image plusImage;
    public Image focusImage;
    public enum CardState { Normal, Focused, Plus }
    public CardState currentState = CardState.Normal;
    public int currentCardId;
    public Color plusColor;

    public void Init(int cardID)
    {
        if(cardID == -1)
        {
            currentCardId = cardID;
            ModeToPlus();
        }
        else if(cardID > -1)
        {
            currentCardId = cardID;
            cardImage.Init(cardID);
            ModeToNormal();
        }
        else
        {
            Debug.LogError($"[SpellCard_Deck] Invalid cardID: {cardID}");
        }
    }
    public void ModeToPlus()
    {
        currentState = CardState.Plus;
        SetCardVisualActive(false);
        plusImage.gameObject.SetActive(true);
        focusImage.gameObject.SetActive(false);
        cardImage.cardFrame.gameObject.SetActive(true);
        cardImage.cardFrame.color = plusColor;
    }
    public void ModeToFocus()
    {
        currentState = CardState.Focused;
        focusImage.gameObject.SetActive(true);
    }
    public void ModeToNormal()
    {
        // 빈 슬롯(-1)은 항상 Plus 상태를 유지해야 한다.
        if (currentCardId == -1)
        {
            ModeToPlus();
            return;
        }

        currentState = CardState.Normal;
        SetCardVisualActive(true);
        plusImage.gameObject.SetActive(false);
        focusImage.gameObject.SetActive(false);
    }

    private void SetCardVisualActive(bool isActive)
    {
        cardImage.mainImage.gameObject.SetActive(isActive);
        cardImage.manaCostImage.gameObject.SetActive(isActive);
        cardImage.manaCost_text.gameObject.SetActive(isActive);
        cardImage.cardFrame.gameObject.SetActive(isActive);
        cardImage.cardDeco.gameObject.SetActive(isActive);
    }
    public void OnClickButton()
    {
        if(currentState == CardState.Normal)
        {
            CallCardInfo();
        }
        else if(currentState == CardState.Plus)
        {
        
        }
        else if(currentState == CardState.Focused)
        {
            if (UICardManager.instance == null || UICardManager.instance.cardInfoManager == null)
            {
                Debug.LogError("[SpellCard_Deck] UICardManager or CardInfoManager is missing.");
                return;
            }

            int deckIndex = Array.IndexOf(UICardManager.instance.spellCard_Decks, this);
            if (deckIndex < 0)
            {
                deckIndex = slotId;
            }

            UICardManager.instance.cardInfoManager.SwapDeckSlot(deckIndex);
        }
    }
    public void CallCardInfo()
    {
        UICardManager.instance.cardInfoManager.gameObject.SetActive(true);
        int level = ServerDataManager.instance.GetCardLevel(currentCardId);
        bool spellCard_Deck = true;//덱 슬롯에서 불리는 카드 정보는 true
        UICardManager.instance.cardInfoManager.Init(currentCardId, level,spellCard_Deck);
    }
}
