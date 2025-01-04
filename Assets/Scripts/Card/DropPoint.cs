using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DropPoint : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image           image;
    private RectTransform   rect;
    public DeckManager deckManager;

    private void Awake()
    {
        rect        = GetComponent<RectTransform>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();
        card.cardReady = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
         if (eventData.pointerDrag != null){
        MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();
        card.cardReady = false;
         }
      
    }
    public void OnDrop(PointerEventData eventData)
    {
        if( eventData.pointerDrag != null )
        {
            //카드 사용 알고리즘
            MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();

             //마나 소모
            float cost = card.cardCost;
            GameManager.instance.player.playerStatus.mana -= cost;

            Debug.Log(card.cardId);
            //eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position =  eventData.pointerDrag.GetComponent<MagicCard>().originalPosition;//원래 위치로

            deckManager.deck.Add(card.cardId);//사용된 카드 덱 맨 아래로
            deckManager.DrawCard(card.fixedCardNumber);

        
        }
       // gameObject.SetActive(false);
    }


    
}