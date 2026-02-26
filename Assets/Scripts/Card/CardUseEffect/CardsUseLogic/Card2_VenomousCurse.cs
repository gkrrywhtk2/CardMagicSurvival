using UnityEngine;
using UnityEngine.EventSystems;

public class Card2_VenomousCurse : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null) return;

        var data = card.cardScritableData;

        // ✅ 레벨 기반 (프로젝트 구조에 맞춰 둘 중 하나 쓰면 됨)
        // int level = card.level;  // 네가 쓰던 필드
        int level = card.currentPlayerCard != null ? card.currentPlayerCard.LEVEL : card.level;

        int splashEffect = 4;
        int poisonZoneEffect = 5;

        GameObject poisonSplash = GameManager.instance.effectPoolManager.Get(splashEffect);
        GameObject poisonZoneObj = GameManager.instance.effectPoolManager.Get(poisonZoneEffect);

        // ✅ Bullet_ZonePoison 사용
        Bullet_ZonePoison poisonZone = poisonZoneObj.GetComponent<Bullet_ZonePoison>();
        if (poisonZone == null) return;

        // 드랍 포인트
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane)
        );
        targetPosition.z = 0;

        poisonSplash.transform.position = targetPosition;
        poisonZoneObj.transform.position = targetPosition;

        // ✅ 크기/지속시간은 그대로(고정 값)
        float zoneDuration = 3f;     // 기존 Uncommon 기준이었는데 "그대로"라 했으니 고정
        float poisonDuration = 10f;  // 기존 그대로 고정

        // ✅ 레벨 기반 데미지 (초당 데미지 기준치)
        float magicDamage = data.GetDamage(level);

        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(
            magicDamage, out bool isCritical
        );

        // poison이 크리 무시 정책이면 isCritical 무시 가능
        poisonZone.Init(finalDamage, zoneDuration, poisonDuration);
    }

    /// <summary>
    /// ✅ 레벨 기반 데미지 공식
    /// - Lv1  : 1.0
    /// - Lv25 : 2.5   (기존 Legendary 최대치에 맞춤)
    /// 선형 증가: 1 + (L-1) * (1.5/24) = 1 + (L-1)*0.0625
    /// </summary>
    private float GetDamageByLevel(int level)
    {
        level = Mathf.Clamp(level, 1, 99);
        return 1f + (level - 1) * 0.0625f; // Lv25 -> 2.5
    }
}
