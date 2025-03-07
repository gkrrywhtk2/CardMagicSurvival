using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;


public class DropPoint : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DeckManager deckManager;
    
     // 카드 효과를 ID로 매핑하기 위한 딕셔너리
    private Dictionary<int, ICardUse> cardAbility;

    private void Awake()
    {
        // gameObject가 비활성화되어 있지 않도록 보장
        gameObject.SetActive(true);
        // 카드 능력 딕셔너리 초기화
        InitCardDictionary();
    }

    public void InitCardDictionary(){
            cardAbility = new Dictionary<int, ICardUse>
        {
            { 0, gameObject.AddComponent<Card0_Haste>() },
            { 1, gameObject.AddComponent<Card1_MeteorStrike>() },
            { 2, gameObject.AddComponent<Card2_VenomousCurse>() },
            { 3, gameObject.AddComponent<Card3_ManaFlow>() },
            { 4, gameObject.AddComponent<Card4_FlameBurst>() },
            { 5, gameObject.AddComponent<Card5_FireBall>() },
              { 6, gameObject.AddComponent<Card6_ArcaneOverDrive>() },
                { 7, gameObject.AddComponent<Card7_Heal>() }
        };
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
        //카드 사용 알고리즘
        MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();
        if(card.cardOn == false)
            return;

        if( eventData.pointerDrag != null )
        {
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
            float cost = card.cardData.cardCost;
            GameManager.instance.player.playerStatus.mana -= cost;

            //카드 사용 로직
            UseCard(eventData);
    
            //사용된 카드 덱 맨 아래로
            deckManager.deck.Add(card.magicCard.ID);
            deckManager.DrawCard(card.fixedCardNumber);
        }
    }


      // 카드 ID에 따라 효과를 실행하는 함수
    public void UseCard(PointerEventData eventData)
    {
        MagicCard card =  eventData.pointerDrag.GetComponent<MagicCard>();
        int ID = card.magicCard.ID;
        int STACK = card.magicCard.STACK;

        if (cardAbility.ContainsKey(ID))
        {
            cardAbility[ID].Use(eventData); // 해당 카드의 효과 실행
        }
        else
        {
            Debug.LogWarning("카드 효과가 정의되지 않았습니다.");
        }
    }    
}