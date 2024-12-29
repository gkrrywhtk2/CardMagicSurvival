using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DropPoint : MonoBehaviour, IPointerEnterHandler, IDropHandler, IPointerExitHandler
{
    private Image           image;
    private RectTransform   rect;


    private void Awake()
    {
        image       = GetComponent<Image>();
        rect        = GetComponent<RectTransform>();
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.yellow;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if( eventData.pointerDrag != null )
        {
            //카드 사용 알고리즘
            eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position = rect.position;
        }
    }
}