using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DropPoint : MonoBehaviour,IDropHandler
{
    private Image           image;
    private RectTransform   rect;
    public DeckManager deckManager;

    private void Awake()
    {
        rect        = GetComponent<RectTransform>();
    }


    public void OnDrop(PointerEventData eventData)
    {
        if( eventData.pointerDrag != null )
        {
            //카드 사용 알고리즘
            Debug.Log( eventData.pointerDrag.GetComponent<MagicCard>().cardId);
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position =  eventData.pointerDrag.GetComponent<MagicCard>().originalPosition;//원래 위치로

            deckManager.deck.Add(eventData.pointerDrag.GetComponent<MagicCard>().cardId);//사용된 카드 덱 맨 아래로
            deckManager.DrawCard(eventData.pointerDrag.GetComponent<MagicCard>().fixedCardNumber);
        
        }
       // gameObject.SetActive(false);
    }


    
}