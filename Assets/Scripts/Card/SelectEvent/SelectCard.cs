using UnityEngine;
using UnityEngine.UI;

public class SelectCard : MonoBehaviour
{
    public Image focus;
    public CardImage cardImage;
    

    public void OnFocusCard()
    {
        focus.gameObject.SetActive(true);
    }
    public void OffFocusCard()
    {
        focus.gameObject.SetActive(false);
    }
}
