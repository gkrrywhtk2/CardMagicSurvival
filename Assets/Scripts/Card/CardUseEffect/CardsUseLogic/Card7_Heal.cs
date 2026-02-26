using UnityEngine;
using UnityEngine.EventSystems;

public class Card7_Heal : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        var data = card.cardScritableData;

        Player_Status status = GameManager.instance.player.playerStatus;
        if (status == null || status.playerHP == null)
            return;

        // ✅ 등급 대신 레벨 사용
        int level = card.currentPlayerCard.LEVEL;

        // 이펙트 연출 (프로젝트 사정상 유지)
        GameManager.instance.player.playerEffect.card6_Effect0.SetActive(true);

        // ✅ 최대 체력 기준 퍼센트 힐 (레벨 기반)
        float healPercent = data.GetHeal(level); // Lv1=10%, Lv25=40%
        float healValue = status.playerHP.maxHealth * (healPercent / 100f);

        // ✅ 체력 회복
        status.playerHP.Heal(healValue);
    }
}
