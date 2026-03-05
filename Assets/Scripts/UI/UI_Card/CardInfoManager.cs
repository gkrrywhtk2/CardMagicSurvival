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
    //레벨업 버튼
    public TMP_Text levelUpCostText;//3000 적혀있는 텍스트
    public Image levelUpCostBg;//레벨업 버튼 배경 이미지 조건이 안되면 활성화
    public Color levelUpCostAvailableColor = Color.black;
    public Color levelUpCostUnavailableColor = Color.red;

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
        SetLevelUpCostUI();
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
        UICardManager.instance.backGroundFocusImage.gameObject.SetActive(true);//배경 어둡게
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
        UICardManager.instance.backGroundFocusImage.gameObject.SetActive(false);//배경 원상복귀
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

    private void SetLevelUpCostUI()
    {
        bool isUpgradeable = spellCardAccountImage != null && spellCardAccountImage.isUpgradeable;
        const int levelUpCost = 3000;
        int nowGold = ServerDataManager.instance != null ? ServerDataManager.instance.GetCurrentGold() : 0;
        bool hasEnoughGold = nowGold >= levelUpCost;
        bool canLevelUp = isUpgradeable && hasEnoughGold;


        if (levelUpCostText != null)
        {
            levelUpCostText.text = levelUpCost.ToString();
            levelUpCostText.color = hasEnoughGold ? levelUpCostAvailableColor : levelUpCostUnavailableColor;
        }

        if (levelUpCostBg != null)
        {
            levelUpCostBg.gameObject.SetActive(!canLevelUp);
        }
    }
    public void OnClickLevelUp()
    {
        if (ServerDataManager.instance == null)
        {
            Debug.LogError("[CardInfoManager] ServerDataManager.instance is null.");
            return;
        }

        var accountCardManager = AccountCardManager.Instance;
        if (accountCardManager == null)
        {
            Debug.LogError("[CardInfoManager] AccountCardManager.Instance is null.");
            return;
        }

        const int levelUpCost = 3000;
        int currentLevel = ServerDataManager.instance.GetCardLevel(currentCardId);
        int currentStock = ServerDataManager.instance.GetCardStock(currentCardId);
        int requiredCards = accountCardManager.GetRequiredCardsForLevelUp(currentLevel);
        int nowGold = ServerDataManager.instance.GetCurrentGold();

        bool hasEnoughCards = currentStock >= requiredCards;
        bool hasEnoughGold = nowGold >= levelUpCost;

        if (!hasEnoughCards)
        {
            Debug.LogWarning($"[CardInfoManager] Not enough card materials. cardId={currentCardId}, stock={currentStock}, required={requiredCards}");
        }

        if (!hasEnoughGold)
        {
            Debug.LogWarning($"[CardInfoManager] Not enough gold to level up. currentGold={nowGold}, requiredGold={levelUpCost}");
        }

        if (!hasEnoughCards || !hasEnoughGold)
        {
            return;
        }

        var accountCards = ServerDataManager.instance.GetListOfAccountSpellCards();
        var targetCard = System.Array.Find(accountCards, c => c != null && c.id == currentCardId);
        if (targetCard == null)
        {
            Debug.LogError($"[CardInfoManager] Account spell card not found. cardId={currentCardId}");
            return;
        }

        targetCard.level += 1;
        targetCard.stock = Mathf.Max(0, targetCard.stock - requiredCards);
        ServerDataManager.instance.AddGold(-levelUpCost);

        if (accountCardManager.mergedCardList != null)
        {
            var merged = accountCardManager.mergedCardList.Find(c => c != null && c.id == currentCardId);
            if (merged != null)
            {
                merged.level = targetCard.level;
                merged.stock = targetCard.stock;
                merged.isUnlocked = targetCard.isUnlocked;
            }
        }

        if (UICardManager.instance != null && UICardManager.instance.spellCard_Buttons != null)
        {
            for (int i = 0; i < UICardManager.instance.spellCard_Buttons.Length; i++)
            {
                var button = UICardManager.instance.spellCard_Buttons[i];
                if (button == null || button.cardId != currentCardId) continue;
                button.Init(currentCardId);
                break;
            }
        }

        Init(currentCardId, targetCard.level, isCurrentCardFromDeck);
        Debug.Log($"[CardInfoManager] Level up success. cardId={currentCardId}, level={targetCard.level}, stock={targetCard.stock}");
    }
}
