using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

namespace Game.InGameCardManager
{

    public class InGameCardManager : MonoBehaviour
{   //역할
    // 1. 게임중 덱을 관리 
    public AccountCardManager accountCardManager; // 참조 연결
    public Queue<int> deck = new Queue<int>();    // 인게임 덱
    public int[] hand = new int[4];               // 고정 핸드

    //오브젝트
    public MagicCard[] magicCards; //카드 오브젝트
    public Image nextcardImage;//다음 카드 이미지


    public void InitDeck()
    {
        // 1.AccountCardManager에서 현재 선택된 덱 슬롯을 가져온다.
        // 2.덱 초기세팅

        List<int> deckIds = accountCardManager.GetDeckSlotIds();
        deck = new Queue<int>(deckIds);
        ShuffleDeck();
        HandInit();
    }


    public void ShuffleDeck()
    {
        // Queue를 List로 변환
        List<int> tempList = deck.ToList();

        // Fisher–Yates 알고리즘으로 섞기
        for (int i = tempList.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (tempList[i], tempList[randomIndex]) = (tempList[randomIndex], tempList[i]);
        }

        // 다시 Queue로 변환
        deck = new Queue<int>(tempList);

        Debug.Log($"[ShuffleDeck] 덱 셔플 완료 → {string.Join(", ", deck)}");
    }

    public void HandInit()
    {
        //덱을 설정한 후 처음 3장 뽑기 
        StartCoroutine(FirstDraw());
    }

    private IEnumerator FirstDraw()
    {

        // **2. 덱에서 4장 핸드로
        int handCount = 4;

        //카드 리로딩 애니메이션
        magicCards[0].CardReload();
        magicCards[1].CardReload();
        magicCards[2].CardReload();
        magicCards[3].CardReload();

        for (int i = 0; i < handCount; i++)
        {
            int cardId = deck.Dequeue(); // 덱 맨 위의 카드 ID 가져오기
           // deck.Enqueue(cardId); //덱 맨위로 올리기

            // 핸드 카드 초기화
            magicCards[i].CardInit(cardId);

            // 0.3초 지연시켜서 순서대로 뽑게끔
            yield return new WaitForSeconds(0.3f);
        }

        // 마지막에 다음 카드 이미지 설정
        //NextCardImageSetting();
    }

    public void DrawCard(int handNumber)
    {
        //핸드에서 카드를 사용한 후 드로우하는 함수
        StartCoroutine(DrawCard_Corutine(handNumber));
    }

    public IEnumerator DrawCard_Corutine(int handNumber)
    {
        int cardId = deck.Dequeue(); // 우선 순위 카드 ID 가져오기
        //deck.Enqueue(cardId);  //맨위로 올리기

        magicCards[handNumber].CardFalse();//카드 숨기기

        //드로우 딜레이
        int basicDrawCoolTime = 1;
        yield return new WaitForSeconds(basicDrawCoolTime); // 1초 기다림

        //카드 데이터 세팅
        magicCards[handNumber].CardInit(cardId);
        //NextCardImageSetting();
    }
    public void NextCardImageSetting()
    {
        int nextCardId = deck.Peek(); // 맨 앞 요소 '참조만'
        nextcardImage.sprite = LocalDataManager.Instance.cardData.cardScritableData[nextCardId].cardImage;

    }
}
}
