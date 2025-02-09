using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class DropPoint : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Image           image;
    private RectTransform   rect;
    public DeckManager deckManager;

    //flameburst 
    public Coroutine flameBurstCorutine;
    public bool flameburstOn;
    public float flameburstTimer;
    public float flameburstDuration = 5;

    private void Awake()
    {
        rect        = GetComponent<RectTransform>();
    }
    private void Update(){
      
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
        MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();
        if(card.cardOn == false)
            return;

        if( eventData.pointerDrag != null )
        {
            //카드 사용 알고리즘
          
            card.CardLock();
            
            //사용 이펙트
            if(card.rangeOn != true){
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane));
                    targetPosition.z = 0; // 2D 환경이라면 Z축을 고정
                    int cardUseEffectNum = 1;
                GameObject effect = GameManager.instance.poolManager.Get(cardUseEffectNum);
                effect.transform.position = targetPosition;
            }
                   
             //마나 소모
            float cost = card.cardCost;
            int rank = card.cardRank;
            GameManager.instance.player.playerStatus.mana -= cost;

            //카드 사용 로직
            CardUse(card.cardId, eventData);
            //eventData.pointerDrag.transform.SetParent(transform);

            //eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = eventData.pointerDrag.GetComponent<MagicCard>().cardDrawStartPosition;//원래 위치로
           
            deckManager.deck.Add(new Card(card.cardId, rank));//사용된 카드 덱 맨 아래로
            deckManager.DrawCard(card.fixedCardNumber);

        
        }
       // gameObject.SetActive(false);
    }
    public void CardUse(int cardId,PointerEventData eventData){
        switch(cardId){
            case 0:
            Card0_Concentration();
            return;

            case 1:
        Card1_FireBall(eventData);
            return;

            case 2:
        Card2_PosionPoison(eventData);
            return;

            case 3:
        Card3_ManaUp();
            return;
            case 4:
            Card4_FlameBurst();
            return;
            default:
            return;
        }
    }

     public void Card0_Concentration(){
         GameManager.instance.player.playerEffect.PlayConcentration();
         StartCoroutine(Concentration());
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

    float damage = GameManager.instance.player.playerStatus.DamageReturn(0.3f,out bool isCritical);
    int per = -2;
    float bulletspeed = 0;
    Vector3 dir = Vector3.zero;
    int effectNumber = -1;
    global::bullet.bulletType type = global::bullet.bulletType.placement;
    posion.Init(damage, per, bulletspeed, dir,effectNumber,type,isCritical);

}

public void Card3_ManaUp(){
    GameManager.instance.player.playerEffect.PlayManaUp();
    StartCoroutine(ManaPlus());

}

    IEnumerator ManaPlus(){

        float duration = 1.5f;
        float plus = (GameManager.instance.player.playerStatus.manaRecovery);
        GameManager.instance.player.playerStatus.manaRecoveryPlus = plus;
        yield return new WaitForSeconds(duration);
        GameManager.instance.player.playerStatus.manaRecoveryPlus  = 0;

    }

    IEnumerator Concentration(){
        float duration = 1.5f;
        float plus = (GameManager.instance.player.autoAttack.autoAttackRecovery);
        GameManager.instance.player.autoAttack.autoAttackRecoveryPlus0 = (plus * 3);
        yield return new WaitForSeconds(duration);
        GameManager.instance.player.autoAttack.autoAttackRecoveryPlus0 = 0;

    }

   public void Card4_FlameBurst()
{
    // 기존 Coroutine이 실행 중이면 중단
    if (flameBurstCorutine != null)
    {
        StopCoroutine(flameBurstCorutine);
    }

    // 상태 초기화
    flameburstOn = true;
    flameburstTimer = 0;

    // 새로운 Coroutine 시작
    flameBurstCorutine = StartCoroutine(FlameBurstRoutine());
}
   private IEnumerator FlameBurstRoutine()
{
    float damage = GameManager.instance.player.playerStatus.DamageReturn(0.3f, out bool isCritical);
    int flameburstObjectNum = 7; // 오브젝트 풀에서 가져올 ID

    while (flameburstOn && flameburstTimer < flameburstDuration)
    {
        flameburstTimer += 0.2f; // 타이머 증가 (루프 주기와 일치)
        
        // FlameBurst 효과 생성
        GameObject flame = GameManager.instance.effectPoolManager.Get(flameburstObjectNum);
        flame.GetComponent<Melee>().Init(damage,isCritical);

        Vector2 skillPosition; //플레이어의 바로 앞 스킬이 연출될 좌표
        float angle;//각도
        //player의 dir_front에서 좌표 가져옴
        skillPosition = GameManager.instance.player.dirFront.skillPosition;
        angle = GameManager.instance.player.dirFront.angle;
        flame.transform.position = skillPosition;
        flame.transform.rotation = Quaternion.Euler(0, 0, angle);
        yield return new WaitForSeconds(0.2f); // 0.2초 간격으로 효과 발생
    }

    // 종료 시 상태 정리
    flameburstOn = false;
    flameburstTimer = 0;
    flameBurstCorutine = null;
}

    
}