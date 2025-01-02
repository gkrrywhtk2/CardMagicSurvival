using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    public Transform canvas;  // 카드가 드래그 중일 때 속할 부모 Transform
    private Transform previousParent; // 드래그 전 원래 부모 Transform
    private RectTransform rect;       // RectTransform 참조
    private CanvasGroup canvasGroup;  // CanvasGroup 참조
    public Vector3 originalPosition; // 카드 원래 위치 저장

    [Header("Card Info")]
    public int cardId;       // 카드 ID
    public int cardCost;     // 카드 비용
    Image cardImage;  // 카드 이미지
    public int fixedCardNumber;//012 어떤 위치의 카드인지

    [Header("Object Connect")]
    public TMP_Text costText;//코스트 숫자

    private void Awake()
    {
        // RectTransform 및 CanvasGroup 초기화
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardImage = GetComponent<Image>();
    }

    /// <summary>
    /// 카드 데이터를 초기화하는 메서드
    /// </summary>
    public void CardInit(CardData data)
    {
        if (data == null)
        {
            Debug.LogError("CardData is null!");
            return;
        }

        cardId = data.cardId;
        cardCost = data.cardCost;
        cardImage.sprite = data.cardImage;
        costText.text = cardCost.ToString();
        
    }

    /// <summary>
    /// 드래그 시작 시 호출
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        previousParent = transform.parent; // 현재 부모 저장
        originalPosition = rect.position;  // 현재 위치 저장
        transform.SetParent(canvas);       // 드래그 중 부모를 Canvas로 설정
        transform.SetAsLastSibling();      // 카드가 최상위에 렌더링되도록 설정

        canvasGroup.alpha = 0.6f;          // 카드 투명도 조정
        canvasGroup.blocksRaycasts = false; // 레이캐스트 막기
    }

    /// <summary>
    /// 드래그 중 호출
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position; // 드래그 위치로 이동
    }

    /// <summary>
    /// 드래그 종료 시 호출
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1.0f;          // 투명도 원복
        canvasGroup.blocksRaycasts = true; // 레이캐스트 다시 활성화

        // 부모가 Canvas인 경우 원래 위치로 복원
        if (transform.parent == canvas)
        {
            transform.SetParent(previousParent);
            rect.position = originalPosition;
        }
    }
}
