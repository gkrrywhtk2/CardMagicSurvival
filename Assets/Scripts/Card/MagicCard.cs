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
    public bool cardOn;

    [Header("Object Connect")]
    public TMP_Text costText;//코스트 숫자
    public Image dropPoint;//드랍 포인트
    public Image CoolTimeImage;//시계 방향 쿨타임 이미지
    public GameObject range;//스킬 범위 이미지 오브젝트
    public bool rangeOn;
    public bool cardReady;//drop 포인트위에 있을때 true
    private bool lastRangeState = false; // 이전 상태를 저장할 변수

    private void Awake()
    {
        // RectTransform 및 CanvasGroup 초기화
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        cardImage = GetComponent<Image>();
    }

    private void Update(){
        ClockCoolTime();
    }

    /// <summary>
    /// 카드 데이터를 초기화하는 메서드
    /// </summary>
    public void CardInit(CardData data)
    {
        cardOn = false;
        cardReady = false;
        if (data == null)
        {
            Debug.LogError("CardData is null!");
            return;
        }

        cardId = data.cardId;
        cardCost = data.cardCost;
        cardImage.sprite = data.cardImage;
        costText.text = cardCost.ToString();
        rangeOn = data.isRangeCard;
        range.GetComponent<RectTransform>().localScale = data.rangeScale;
        
        
    }

    /// <summary>
    /// 드래그 시작 시 호출
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
            if(cardOn != true)
                return;
            if(GameManager.instance.cardOneTouch != true)
                return;

        GameManager.instance.cardOneTouch = true;
        previousParent = transform.parent; // 현재 부모 저장
        originalPosition = rect.position;  // 현재 위치 저장
        transform.SetParent(canvas);       // 드래그 중 부모를 Canvas로 설정
        transform.SetAsLastSibling();      // 카드가 최상위에 렌더링되도록 설정

        canvasGroup.alpha = 0.6f;          // 카드 투명도 조정
        canvasGroup.blocksRaycasts = false; // 레이캐스트 막기
        dropPoint.raycastTarget = true;//드롭 포인트 활성화
    }

    /// <summary>
    /// 드래그 중 호출
    /// </summary>
    public void OnDrag(PointerEventData eventData)
{
    // 카드가 활성화된 상태일 때만 드래그를 진행
    if (cardOn != true)
        return;

    // 드래그 위치로 이동
    rect.position = eventData.position;

    // 카드가 범위 카드인지, 그리고 dropPoint 위에 있는지 확인
    bool shouldRangeBeActive = (rangeOn == true && cardReady == true);

    // 범위 이미지의 활성 상태가 변경되었을 때만 SetActive 호출
    if (shouldRangeBeActive != lastRangeState)
    {
        range.gameObject.SetActive(shouldRangeBeActive);
        lastRangeState = shouldRangeBeActive;  // 상태 갱신
    }
}

    /// <summary>
    /// 드래그 종료 시 호출
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {

        GameManager.instance.cardOneTouch = false;
        canvasGroup.alpha = 1.0f;          // 투명도 원복
        canvasGroup.blocksRaycasts = true; // 레이캐스트 다시 활성화

        // 부모가 Canvas인 경우 원래 위치로 복원
        if (transform.parent == canvas)
        {
            transform.SetParent(previousParent);
            rect.position = originalPosition;
        }
        dropPoint.raycastTarget = false;//드롭 포인트 활성화
        range.gameObject.SetActive(false);//범위 이미지 비활성화
    }

    public void ClockCoolTime(){
     // 현재 마나와 카드 비용 비율 계산
    float mana = GameManager.instance.player.playerStatus.mana;
    float value = Mathf.Clamp01(mana / cardCost);

    // 쿨타임 UI 업데이트
    CoolTimeImage.fillAmount = value;

    // 카드 사용 가능 여부 업데이트
    bool previousState = cardOn; // 이전 상태 저장
    cardOn = value >= 1;

    // 상태가 변경된 경우만 투명도 조정
    
        SetCardVisibility(cardOn);
    
}
// 카드의 투명도 조정
private void SetCardVisibility(bool isCardOn)
{
    Color currentColor = CoolTimeImage.color;

    if (isCardOn) {
        // 카드가 사용 가능하면 투명도 0 (안보이게)
        currentColor.a = 0f;
    } else {
        // 카드가 사용 불가능하면 투명도 1 (보이게)
        currentColor.a = 0.8f;
    }

    CoolTimeImage.color = currentColor;
}

}

