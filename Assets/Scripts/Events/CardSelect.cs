using UnityEngine;
using System.Collections.Generic;


public class CardSelect : MonoBehaviour
{
    public GameObject[] selectCards; //012 선택 카드

    public void EventStart()
    {
       //카드 선택 이벤트를 시작합니다.
        Time.timeScale = 0; //게임 일시정지
        
        // ✅ 전체 카드풀 가져오기
        List<PlayerCard> allCards = AccountCardManager.Instance.GetAccountCardPool();
        Debug.Log($"[CardSelect] 보유 카드 수: {allCards.Count}개");

        // ✅ 랜덤으로 3장 뽑기
        List<PlayerCard> randomCards = GetRandomCards(allCards, 3);
        
        Debug.Log("<color=#FFFF00>==== 랜덤으로 뽑힌 3장 ====</color>");
        
        // ✅ 수정: for 루프로 인덱스 사용
        for (int i = 0; i < randomCards.Count; i++)
        {
            selectCards[i].GetComponent<CardImage>().Init(randomCards[i].cardId);
            Debug.Log($"<color=#00FF00>[Random Card {i}] 카드ID: {randomCards[i].cardId}, 등급: {randomCards[i].currentRarity}, 보유량: {randomCards[i].quantity}</color>");
        }
    }
    
    // 랜덤으로 n장 뽑는 함수 (중복 없음)
    private List<PlayerCard> GetRandomCards(List<PlayerCard> cardPool, int count)
    {
        if (cardPool.Count < count)
        {
            Debug.LogWarning($"[CardSelect] 카드풀({cardPool.Count}장)이 요청된 개수({count}장)보다 적습니다!");
            return new List<PlayerCard>(cardPool); // 전체 반환
        }

        // 복사본 만들어서 섞기
        List<PlayerCard> shuffled = new List<PlayerCard>(cardPool);
        
        // Fisher-Yates 셔플
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (shuffled[i], shuffled[randomIndex]) = (shuffled[randomIndex], shuffled[i]);
        }

        // 앞에서 count개 가져오기
        return shuffled.GetRange(0, count);
    }
    
    public void EventEnd()
    {
        Time.timeScale = 1; //다시 시작
        gameObject.SetActive(false);
    }
}