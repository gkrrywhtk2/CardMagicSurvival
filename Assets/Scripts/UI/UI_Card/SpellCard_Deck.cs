using UnityEngine;

public class SpellCard_Deck : MonoBehaviour
{
    public CardImage cardImage;

    public void Init(int cardID)
    {
        cardImage.Init(cardID);
    }
}
