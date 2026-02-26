using UnityEngine;
using UnityEngine.EventSystems;

public class Card3_ManaFlow : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null)
            return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null)
            return;

        var data = card.cardScritableData;

        // ✅ 등급(Rank) 대신 레벨(Level)
        int level = card.currentPlayerCard.LEVEL;

        // 이펙트
        GameManager.instance.player.playerEffect.PlayManaUp();

        // ✅ 지속시간 고정
        float duration = data.GetDuration(level); // 레벨 기반 지속시간 (원하면 나중에 공식화 가능)

        // ✅ 레벨 기반 마나 회복량
        // 요구: Lv25쯤에서 체감 있게 증가시키기 (아래 공식: Lv1=1, Lv25=4)
        float bonusFlatRecovery = 2;

        // PlayerMana 가져오기
        PlayerMana mana = GameManager.instance.player.playerStatus.playerMana;
        if (mana == null)
            return;

        // ✅ "추가 회복량"을 시간제로 추가 (중첩 가능, 각자 만료)
        mana.AddBonusManaRecoveryFlatTimed(bonusFlatRecovery, duration);
    }

}
