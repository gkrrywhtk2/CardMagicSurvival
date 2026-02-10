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
}
