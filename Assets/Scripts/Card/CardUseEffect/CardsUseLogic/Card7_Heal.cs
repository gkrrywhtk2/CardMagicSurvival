using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card7_Heal : MonoBehaviour, ICardUse
{

   public void Use(PointerEventData eventData)
{
    MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
    Player_Status player = GameManager.instance.player.playerStatus;

    // 이펙트 연출
    GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);

    // 회복량 가져오기 (heal_per이 %단위라면)
    float heal_per = GameManager.instance.deckManager.cardDatas[card.magicCard.ID]
        .GetHeal(card.magicCard.STACK);
    float healValue = player.maxHealth * (heal_per / 100f); //퍼센트 회복 계산

    // 체력 회복 (최대 체력 초과 방지)
    player.health = Mathf.Min(player.health + healValue, player.maxHealth);
}

}
