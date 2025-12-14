using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Linq; // LINQ 사용
using Game.RankSystem;

public class MagicCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag Settings")]
    public Transform canvas;  // 카드가 드래그 중일 때 속할 부모 Transform
    private Transform previousParent; // 드래그 전 원래 부모 Transform
    private RectTransform rect;       // RectTransform 참조
    private CanvasGroup canvasGroup;  // CanvasGroup 참조
    public Vector3 originalPosition; // 카드 원래 위치 저장
    Animator anim;
    public bool oneTimeDrawAnim;
    public Vector3 cardDrawStartPosition;//카드 드로우 연출시 시작하는 위치

    [Header("Card Info")]
    public CardScritableData cardScritableData;
    //public Card magicCard;
    public PlayerCard currentPlayerCard; // ✅ 현재 카드 정보
    public int id;
    public int fixedCardNumber;//012 어떤 위치의 카드인지
    public bool cardOn;//코스트 체크
    public bool cardDrawLock;//카드 드로우 애니메이션 연출시 카드 터치 금지용

    public Image dropPoint;//드랍 포인트
    public Image CoolTimeImage;//시계 방향 쿨타임 이미지
    public GameObject range;//스킬 범위 이미지 오브젝트
    public bool rangeOn;
    public bool directionCard;
    public bool cardReady;//drop 포인트위에 있을때 true
    private bool lastRangeState = false; // 이전 상태를 저장할 변수
    private bool lastCardReadyState = false;
    public Image manaCost;//마나 보석 이미지
   // public TMP_Text cardLevelText;//카드 레벨 텍스트
    public DIr_FrontForCard dIr_FrontForCard;
    private CardImage cardImage;

    private void Awake()
    {
        // RectTransform 및 CanvasGroup 초기화
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rect.position;
        anim = GetComponent<Animator>();
        cardDrawLock = false;
        cardImage = GetComponent<CardImage>();
    }

    private void Update()
    {
        ClockCoolTime();
    }

    /// <summary>
    /// 카드 데이터를 초기화하는 메서드
    /// </summary>

    public void CardReload()
    {
        anim.enabled = false;//애니메이션 비활성화하여 앵커드 포지션 적용되게 변경
        rect.anchoredPosition = new Vector3(-5000, -5000, 0);
        // Debug.Log("위치 조정");
    }
    public void CardFalse()
    {
        //애니메이션 비활성화하여 앵커드 포지션 적용되게 변경
        anim.enabled = false;
        rect.anchoredPosition = new Vector3(-5000, -5000, 0);
    }
    // ✅ PlayerCard를 받도록 수정
    public void CardImageInit(PlayerCard playerCard)
    {
        cardImage.Init(playerCard); // PlayerCard 전달
    }
    public void CardInit(PlayerCard playerCard)
    {
        currentPlayerCard = playerCard; // ✅ 참조 저장
        id = currentPlayerCard.ID;
        cardScritableData = LocalDataManager.Instance.cardData.cardScritableData[id];
        cardOn = false;
        cardReady = false;

        // ✅ PlayerCard를 전달! (등급 정보 포함)
        CardImageInit(currentPlayerCard);

        rangeOn = cardScritableData.isRangeCard;
        range.GetComponent<RectTransform>().localScale = cardScritableData.rangeScale_Card;
        directionCard = cardScritableData.isDirCard;
        cardImage.CardAlpha1_Range();
        CardDrawAni(0);
    }

    /// <summary>
    /// 드래그 시작 시 호출
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
         //Debug.Log($"OnBeginDrag called for {gameObject.name}");
         /**
            if(cardOn != true){
                eventData.pointerDrag = null; // 카드가 비활성화 상태라면 드래그 호출 차단
                return;
            }
        **/
            if(GameManager.instance.GamePlayState != true){
                eventData.pointerDrag = null; 
                return;
            }
            if(GameManager.instance.ItemSelectState == true){
                eventData.pointerDrag = null; 
                return;
            }
        
            if(GameManager.instance.cardOneTouch == true)
                return;

        GameManager.instance.cardOneTouch = true;
        previousParent = transform.parent; // 현재 부모 저장
        transform.SetParent(canvas);       // 드래그 중 부모를 Canvas로 설정
        transform.SetAsLastSibling();      // 카드가 최상위에 렌더링되도록 설정

        canvasGroup.alpha = 0.6f;          // 카드 투명도 조정
        canvasGroup.blocksRaycasts = false; // 레이캐스트 막기
        dropPoint.raycastTarget = true;//드롭 포인트 활성화

        //방향 카드라면 화살표 활성화
        if(directionCard == true)
            dIr_FrontForCard.gameObject.SetActive(true);
    }

    /// <summary>
    /// 드래그 중 호출
    /// </summary>
    public void OnDrag(PointerEventData eventData)
{
    // 카드가 활성화된 상태일 때만 드래그를 진행
    /**
            if(cardOn != true){
                 eventData.pointerDrag = null; // // 카드가 비활성화 상태라면 드래그 호출 차단
                return;
            }
            **/
            if(GameManager.instance.GamePlayState != true){
                eventData.pointerDrag = null; 
                return;
            }
            if(GameManager.instance.ItemSelectState == true){
                eventData.pointerDrag = null; 
                return;
            }
            

    // 드래그 위치로 이동
    rect.position = eventData.position;
      anim.enabled = false; // 드래그 시작 시 애니메이션 멈춤

    // 카드가 범위 카드인지, 그리고 dropPoint 위에 있는지 확인, 그리고 카드가 활성화 상태인지
    bool shouldRangeBeActive = (rangeOn == true && cardReady == true && cardOn == true);
    bool shouldCardReadyBeActive = (cardReady == true);

    if(shouldCardReadyBeActive != lastCardReadyState){
        // cardLevelText.gameObject.SetActive(shouldCardReadyBeActive);
         lastCardReadyState = shouldCardReadyBeActive;  // 상태 갱신
    }

    // 범위 이미지의 활성 상태가 변경되었을 때만 SetActive 호출
    if (shouldRangeBeActive != lastRangeState)
    {
        cardImage.CardAlpha0_Range(shouldRangeBeActive);
        range.gameObject.SetActive(shouldRangeBeActive);
        lastRangeState = shouldRangeBeActive;  // 상태 갱신
    }

    //사용한 위치로 날아가는 효과가 필요한 경우
    if (!dIr_FrontForCard.gameObject.activeSelf)
    return;
    Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector2(eventData.position.x, eventData.position.y));
    dIr_FrontForCard.ArrowDirSetting(targetPosition);
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
        }
        dropPoint.raycastTarget = false;//드롭 포인트 활성화
        range.gameObject.SetActive(false);//범위 이미지 비활성화
        cardImage.CardAlpha1_Range();

        if(cardReady != true || cardOn != true)//카드가 사용되어지지 않았다면 기존 위치로 복귀
            anim.enabled = true; // 드래그 중지 시 애니메이션 연출 가능
        
       // Debug.Log(cardReady);

        dIr_FrontForCard.gameObject.SetActive(false);//방향 오브젝트 비활성화

    }

    public void ClockCoolTime()
    {
        if(cardScritableData == null)
        return;//카드가 생성되기전에는 실행 X

        // 현재 마나와 카드 비용 비율 계산
        float mana = GameManager.instance.player.playerStatus.mana;
        float value = Mathf.Clamp01(mana / cardScritableData.cardCost);
    
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
    public void CardDrawAni(float coolTime){
        anim.enabled = true;//애니메이션 활성화
        StartCoroutine(DrawAnimation(coolTime));
           // anim.ResetTrigger("Draw"); // 기존 트리거 초기화
          //  anim.SetTrigger("Draw");   // 다시 트리거 발동
    }
    IEnumerator DrawAnimation(float coolTime){
        anim.ResetTrigger("Draw"); // 기존 트리거 초기화
        anim.SetTrigger("Draw");   // 다시 트리거 발동
        anim.speed = 0;
        yield return new WaitForSeconds(coolTime);//드로우 쿨타임
        anim.speed = 2;
    }

     IEnumerator AfterDeckSettingCardDraw(float coolTime){
        anim.ResetTrigger("Draw"); // 기존 트리거 초기화
        anim.SetTrigger("Draw");   // 다시 트리거 발동
        anim.speed = 0;
        yield return new WaitForSeconds(coolTime);//드로우 쿨타임
        anim.speed = 2;
    }


    public void CardLock(){
        StartCoroutine(CardLockCorutine());
    }
    IEnumerator CardLockCorutine(){
        cardDrawLock = true; 
        yield return new WaitForSeconds(1);
        cardDrawLock = false;
    }
    public void CardDescUiSeind(){
        //deckmanager한테 카드 정보 보내어, ui에 카드 설명창 연출
        //GameManager.instance.deckManager.CardDescInit(magicCard.ID); 삭제 예정
    }
    public void CardDescUiOff(){
        //GameManager.instance.deckManager.CardDescUi.gameObject.SetActive(false);
    }

    // ✅ 등급 색상 업데이트
    public void RefreshRankColor(RankType rank)
    {
        if (cardImage != null)
        {
            cardImage.UpdateFrameColor(rank);
        }
    }
}



