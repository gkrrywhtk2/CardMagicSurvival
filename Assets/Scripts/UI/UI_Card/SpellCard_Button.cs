using UnityEngine;

public class SpellCard_Button : MonoBehaviour
{
    public CardImage cardImage;
    public SpellCardAccountImage spellCardAccountImage;
    public int cardId;
    
    public void Init(int id)
    {
        this.cardId = id;
        cardImage.Init(id);
        spellCardAccountImage.Init(id);
    }
    public void CallCardInfo()
    {
        UICardManager.instance.cardInfoManager.gameObject.SetActive(true);
        int level = ServerDataManager.instance.GetCardLevel(cardId);
        bool spellCard_Deck = false;//모든 카드 정보는 false
        UICardManager.instance.cardInfoManager.Init(cardId, level,spellCard_Deck);
    }
}
