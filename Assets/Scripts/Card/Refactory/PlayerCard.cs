using RANK;
using UnityEngine;
using Game.RankSystem;

[System.Serializable]
public class PlayerCard
{
    // 1.플레이어 개개인의 진행 상태(등급, 레벨, 보유 수량) 등을 담음.
    // 2.저장/로드 대상이 되는 기초 데이터.
    // 3.카드를 사용하는 오브젝트는 이곳에서 현재 플레이어의 카드의 레벨을 참조한다.

    public int cardId;               // 카드 고유id
    public RankType currentRarity;     // 현재 등급
    public int quantity;             // 보유 수량
    public bool islocked;          // 획득 여부

    public PlayerCard(int id)
    {
        cardId = id;
        currentRarity = RankType.Uncommon;
        quantity = 0;
        islocked = false;
    }
}
