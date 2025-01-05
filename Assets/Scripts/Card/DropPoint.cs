using UnityEngine;
using UnityEngine.Analytics;
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

            //카드 사용 로직
            CardUse(card.cardId, eventData);
            //eventData.pointerDrag.transform.SetParent(transform);
            eventData.pointerDrag.GetComponent<RectTransform>().position =  eventData.pointerDrag.GetComponent<MagicCard>().originalPosition;//원래 위치로

            deckManager.deck.Add(card.cardId);//사용된 카드 덱 맨 아래로
            deckManager.DrawCard(card.fixedCardNumber);

        
        }
       // gameObject.SetActive(false);
    }
    public void CardUse(int cardId,PointerEventData eventData){
        switch(cardId){
            case 0:
            return;

            case 1:
        Card1_FireBall(eventData);
            return;

            case 2:
        Card2_PosionPoison(eventData);
            return;
            default:
            return;
        }
    }
   public void Card1_FireBall(PointerEventData eventData)
{
    // 화염구 생성
    int fireBallNum = 2;
    GameObject fireball = GameManager.instance.poolManager.Get(fireBallNum); 
    FireBall fire = fireball.GetComponent<FireBall>();

       // 생성 위치 설정
    Vector3 startPosition = GameManager.instance.player.fireBallPoint.transform.position;
    fireball.transform.position = startPosition;
    
    // 드랍 포인트를 목표로 초기화
    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
    targetPosition.z = 0; // 2D 환경이라면 Z축을 고정

    fire.Initialize(targetPosition);

   // 방향 벡터 계산
    Vector3 direction = targetPosition - startPosition;

    // 회전 각도 계산 (Atan2 사용)
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    // 화염구 회전 적용
    fireball.transform.rotation = Quaternion.Euler(0f, 0f, angle);
}

public void Card2_PosionPoison(PointerEventData eventData){
     int splashEffect = 4;
      int poisonEffect = 5;
     GameObject poisonSplash = GameManager.instance.effectPoolManager.Get(splashEffect); 
     GameObject poison_temp = GameManager.instance.effectPoolManager.Get(poisonEffect); 
     bullet posion = poison_temp.GetComponent<bullet>();

    

      // 드랍 포인트를 목표로 초기화
    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
    targetPosition.z = 0; // 2D 환경이라면 Z축을 고정

    poisonSplash.transform.position = targetPosition;
    posion.transform.position = targetPosition;

    float damage = 3;
    int per = -2;
    float bulletspeed = 0;
    Vector3 dir = Vector3.zero;
    int effectNumber = -1;
    global::bullet.bulletType type = global::bullet.bulletType.placement;
    posion.Init(damage, per, bulletspeed, dir,effectNumber,type);

}



    
}