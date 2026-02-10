using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UIs")]
    public GameObject lobby;
    public GameObject heroCard;
    public GameObject UI_Card;
    public HeroInfo heroInfo;

    [Header("Manager")]
    public UICardManager uiCardManager;
    public void Open_UI_Card()
    {
        UI_Card.SetActive(true);
        heroInfo.gameObject.SetActive(false);
        lobby.SetActive(false);
        heroCard.SetActive(false);
        uiCardManager.InitSpellCardButtons();

    }
     public void Open_HeroCard()
    {
        heroCard.SetActive(true);
        UI_Card.SetActive(false);
        heroInfo.gameObject.SetActive(false);
        lobby.SetActive(false);
    }
     public void Open_Lobby()
    {
        heroCard.SetActive(false);
        UI_Card.SetActive(false);
        heroInfo.gameObject.SetActive(false);
        lobby.SetActive(true);
    }
}
