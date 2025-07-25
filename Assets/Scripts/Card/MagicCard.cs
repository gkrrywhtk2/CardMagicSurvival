using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Linq; // LINQ 사용

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
    public CardData cardData;
   public Card magicCard;
    //public int cardId;       // 카드 ID
   // public int cardCost;     // 카드 비용
    //public int cardRank;//{1,2,3}
    public int fixedCardNumber;//012 어떤 위치의 카드인지
    public bool cardOn;//코스트 체크
    public bool cardDrawLock;//카드 드로우 애니메이션 연출시 카드 터치 금지용

    [Header("Object Connect")]
     public Image cardImage;  // 카드 이미지


   // public Image[] stars;// 카드 등급 이미지 개발 중단
    //public Sprite star_True;
    //public Sprite star_False;

    public TMP_Text costText;//코스트 숫자
    public Image dropPoint;//드랍 포인트
    public Image CoolTimeImage;//시계 방향 쿨타임 이미지
    public GameObject range;//스킬 범위 이미지 오브젝트
    public bool rangeOn;
    public bool directionCard;
    public bool cardReady;//drop 포인트위에 있을때 true
    private bool lastRangeState = false; // 이전 상태를 저장할 변수
    private bool lastCardReadyState = false;
    public Image manaCost;//마나 보석 이미지
    public TMP_Text cardLevelText;//카드 레벨 텍스트
    public DIr_FrontForCard dIr_FrontForCard;
    

    private void Awake()
    {
        // RectTransform 및 CanvasGroup 초기화
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        //cardImage = GetComponent<Image>();
        originalPosition = rect.position;
        anim = GetComponent<Animator>();
        cardDrawLock = false;
       
   
    }

    private void Update(){
        ClockCoolTime();
    }

    /// <summary>
    /// 카드 데이터를 초기화하는 메서드
    /// </summary>
    
   public void CardReload(){
    anim.enabled = false;//애니메이션 비활성화하여 앵커드 포지션 적용되게 변경
    rect.anchoredPosition = new Vector3(-5000, -5000, 0);
   // Debug.Log("위치 조정");
   }
    public void CardInit(int cardid)
    {
        cardData = GameManager.instance.deckManager.cardDatas[cardid];
        magicCard = GameManager.instance.dataManager.havedCardsList.FirstOrDefault(card => card.ID == cardid);
        cardOn = false;
        cardReady = false;
        cardLevelText.gameObject.SetActive(true);
        cardLevelText.text = "Lv." + magicCard.STACK.ToString();
        cardLevelText.gameObject.SetActive(false);//우선 감추기
        //RankImageSetting(cardRank); 별 연출 개발 중단
        //cardCost = cardData.cardCost;
        cardImage.sprite = cardData.cardImage;
        costText.text = cardData.cardCost.ToString();
        rangeOn = cardData.isRangeCard;
        range.GetComponent<RectTransform>().localScale = cardData.rangeScale_Card;
        directionCard = cardData.isDirCard;
        CardAlpha1_Range();
        CardDrawAni(0);
          //StartCoroutine(CardDrawAnimation());
    }

    public void CardInitWhenStackUpgrade(){
        int id = magicCard.ID;
        cardLevelText.gameObject.SetActive(true);
        cardLevelText.text = "Lv." + magicCard.STACK.ToString();
        cardLevelText.gameObject.SetActive(false);//우선 감추기
    }
    /**
    public void Init_CardUpgrade(int rank){
        cardRank = rank;
     //   RankImageSetting(cardRank); 별 연출 개발 중단

    }
    *///

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
        cardLevelText.gameObject.SetActive(shouldCardReadyBeActive);
         lastCardReadyState = shouldCardReadyBeActive;  // 상태 갱신
    }

    // 범위 이미지의 활성 상태가 변경되었을 때만 SetActive 호출
    if (shouldRangeBeActive != lastRangeState)
    {
        CardAlpha0_Range(shouldRangeBeActive);
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
        CardAlpha1_Range();

        if(cardReady != true || cardOn != true)//카드가 사용되어지지 않았다면 기존 위치로 복귀
            anim.enabled = true; // 드래그 중지 시 애니메이션 연출 가능
        
       // Debug.Log(cardReady);

        dIr_FrontForCard.gameObject.SetActive(false);//방향 오브젝트 비활성화

    }

    public void ClockCoolTime(){
        if(cardData == null)
        return;//카드가 생성되기전에는 실행 X

     // 현재 마나와 카드 비용 비율 계산
    float mana = GameManager.instance.player.playerStatus.mana;
    float value = Mathf.Clamp01(mana / cardData.cardCost);
 
    

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

public void CardAlpha0_Range(bool shouldRangeBeActive){
    //카드 드래그시 카드가 범위 카드라면 카드를 투명화
    if(shouldRangeBeActive == true){
        Color cardColor = cardImage.color;
        Color manaColor = manaCost.color;
        Color textColor = costText.color;
       // Color starColor0 = stars[0].color; 개발 중단
       // Color starColor1 = stars[1].color;
      //  Color starColor2 = stars[2].color;

        cardColor.a = 0;
        manaColor.a = 0;
        textColor.a = 0;
       // starColor0.a = 0; 개발 중단]
      //  starColor1.a = 0;
       // starColor2.a = 0;

        cardImage.color = cardColor;
        manaCost.color = manaColor;
        costText.color = textColor;
       // stars[0].color = starColor0; 개발 중단
      //  stars[1].color = starColor1;
      //  stars[2].color = starColor2;
        //카드 레벨 이미지도 투명화 
    }else{
       CardAlpha1_Range();
    }
}
public void CardAlpha1_Range(){

       Color cardColor = cardImage.color;
        Color manaColor = manaCost.color;
        Color textColor = costText.color;
      //  Color starColor0 = stars[0].color; 개발 중단
      //  Color starColor1 = stars[1].color;
     //  Color starColor2 = stars[2].color;

        cardColor.a = 1;
        manaColor.a = 1;
        textColor.a = 1;
       // starColor0.a = 1; 개발 중단
       // starColor1.a = 1;
       // starColor2.a = 1;


        cardImage.color = cardColor;
        manaCost.color = manaColor;
        costText.color = textColor;
       // stars[0].color = starColor0; 개발 중단
      //  stars[1].color = starColor1;
       // stars[2].color = starColor2;
    }

    /**
    public void RankImageSetting(int rank){
        /
    //카드 등급 이미지(별) 세팅 개발 중단

    //초기화
    //stars[0].sprite = star_False; 개발 중단
   // stars[1].sprite = star_False;
   // stars[2].sprite = star_False;

        switch(rank){
            case 1:
                stars[0].sprite = star_True;
            break;

            case 2:
              stars[0].sprite = star_True;
              stars[1].sprite = star_True;
            break;

            case 3:
               stars[0].sprite = star_True;
               stars[1].sprite = star_True;
               stars[2].sprite = star_True;
            break;

            default:
            break;
        }
    }
    **/
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
        GameManager.instance.deckManager.CardDescInit(magicCard.ID);
    }
    public void CardDescUiOff(){
        GameManager.instance.deckManager.CardDescUi.gameObject.SetActive(false);
    }
}



