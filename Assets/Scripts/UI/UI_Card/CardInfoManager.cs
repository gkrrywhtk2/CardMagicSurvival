using System.Collections.Generic;
using Assets.PixelFantasy.PixelTileEngine.Scripts;
using Game.CardData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardInfoManager : MonoBehaviour
{
    public TMP_Text desc_sub;
    public TMP_Text desc_main;
    public TMP_Text nameText;
    public TMP_Text lvText;
    public CardImage cardImage;
    public SpellCardAccountImage spellCardAccountImage;
    PlayerCard playerCard;
    public RankLabel rankLabel;
    public Button selectButton;
    public Button unslotButton;
    public int currentCardId;
    private bool isCurrentCardFromDeck;

    public void Init(int cardId, int currentLevel, bool isSpellCard_Deck)
    {
        currentCardId = cardId;
        isCurrentCardFromDeck = isSpellCard_Deck;
        InitDescText(cardId, currentLevel);
        playerCard = new PlayerCard(cardId, currentLevel);
        var data = CardData.Instance.cardScritableData[cardId];
        rankLabel.SetRank(data.rank);
        cardImage.Init(playerCard);
        spellCardAccountImage.Init(cardId);
        InitLvText(currentLevel);
        ButtonSetting(cardId,isSpellCard_Deck);
    }
    public void InitLvText(int level)
    {
        lvText.text = "Lv. " + level;
    }
   
    public void InitDescText(int cardId, int currentLevel)
    {
        var data = CardData.Instance.cardScritableData[cardId];
        if (data == null)
        {
            Debug.LogError($"CardData with ID {cardId} not found!");
            return;
        }

        // 메인 설명 갱신 (변수 포함)
        desc_main.text = data.GetParsedDescription(currentLevel);

        // ✅ 서브 설명 갱신 (순수 텍스트)
        if (desc_sub != null)
        {
            desc_sub.text = data.GetParsedSubDescription();
        }
        if(nameText != null)
        {
            nameText.text = data.GetName();
        }
    }

    public void OnSelectButton()
    {
        // 카드 선택 시 처리할 로직 추가
        for(int i = 0; i < UICardManager.instance.spellCard_Decks.Length; i++)
        {
            UICardManager.instance.spellCard_Decks[i].ModeToFocus();
        }
        this.gameObject.SetActive(false);

    }
    public void OnunslotButton()
    {
        var accountCardManager = AccountCardManager.Instance;
        if (accountCardManager == null)
        {
            Debug.LogError("[CardInfoManager] AccountCardManager.Instance is null.");
            return;
        }

        List<ServerDataManager.DeckSlot> deckSlots = accountCardManager.accountDeckSlots;
        if (deckSlots == null || deckSlots.Count == 0)
        {
            Debug.LogWarning("[CardInfoManager] deck slot data is empty.");
            return;
        }

        int slotIndex = deckSlots.FindIndex(slot => slot.ID == currentCardId);
        if (slotIndex < 0)
        {
            Debug.LogWarning($"[CardInfoManager] cardId {currentCardId} is not in any deck slot.");
            return;
        }

        deckSlots[slotIndex].ID = -1;
        accountCardManager.SyncDeckSlotsToServerData();

        if (UICardManager.instance != null)
        {
            UICardManager.instance.InitDecks();
        }

        gameObject.SetActive(false);
    }
    public void SwapDeckSlot(int slotId)
    {
        var accountCardManager = AccountCardManager.Instance;
        if (accountCardManager == null)
        {
            Debug.LogError("[CardInfoManager] AccountCardManager.Instance is null.");
            return;
        }

        List<ServerDataManager.DeckSlot> deckSlots = accountCardManager.accountDeckSlots;
        if (deckSlots == null || deckSlots.Count == 0)
        {
            Debug.LogWarning("[CardInfoManager] deck slot data is empty.");
            return;
        }

        if (slotId < 0 || slotId >= deckSlots.Count)
        {
            Debug.LogError($"[CardInfoManager] invalid slotId: {slotId}");
            return;
        }

        if (isCurrentCardFromDeck)
        {
            Debug.LogWarning("[CardInfoManager] select flow is only available for collection cards.");
            RefreshDeckUI();
            return;
        }

        int currentSlotIndex = deckSlots.FindIndex(slot => slot.ID == currentCardId);
        if (currentSlotIndex == slotId)
        {
            RefreshDeckUI();
            return;
        }

        if (currentSlotIndex >= 0)
        {
            deckSlots[currentSlotIndex].ID = -1;
        }

        deckSlots[slotId].ID = currentCardId;
        accountCardManager.SyncDeckSlotsToServerData();
        RefreshDeckUI();
    }

    private void RefreshDeckUI()
    {
        if (UICardManager.instance == null)
        {
            return;
        }

        UICardManager.instance.InitDecks();
    }
    public void ButtonSetting(int cardId, bool isSpellCard_Deck)
    {
        List<int> deckSlotIds = AccountCardManager.Instance != null
            ? AccountCardManager.Instance.GetDeckSlotIds()
            : new List<int>();

        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(!isSpellCard_Deck);
        }

        if (unslotButton != null)
        {
            unslotButton.gameObject.SetActive(isSpellCard_Deck);
        }
    }
}
