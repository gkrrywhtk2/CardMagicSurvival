using Assets.PixelFantasy.PixelTileEngine.Scripts;
using Game.CardData;
using TMPro;
using UnityEngine;

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

    public void Init(int cardId, int currentLevel)
    {
        InitDescText(cardId, currentLevel);
        playerCard = new PlayerCard(cardId, currentLevel);
        var data = CardData.Instance.cardScritableData[cardId];
        rankLabel.SetRank(data.rank);
        cardImage.Init(playerCard);
        spellCardAccountImage.Init(cardId);
        InitLvText(currentLevel);
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
}
