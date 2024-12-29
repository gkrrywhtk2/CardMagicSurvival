using UnityEngine;
using UnityEngine.EventSystems;

public class Darg : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform               canvas;
    private Transform               previousParent;
    private RectTransform           rect;
    private CanvasGroup             canvasGroup;
    private Vector3 originalPosition;

    private void Awake()
    {
        rect        = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent;
        originalPosition = rect.position; // 원래 위치 저장
        transform.SetParent(canvas);
        transform.SetAsLastSibling();

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(transform.parent == canvas)
        {
            transform.SetParent(previousParent);
            rect.position = originalPosition; // 저장된 위치로 복원
        }

        canvasGroup.alpha = 1.0f;
        canvasGroup.blocksRaycasts = true;
    }
}