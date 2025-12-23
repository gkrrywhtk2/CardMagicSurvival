using UnityEngine;
using UnityEngine.EventSystems;
using Game.RankSystem;

public class Card2_VenomousCurse : MonoBehaviour, ICardUse
{
    public void Use(PointerEventData eventData)
    {
        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        var rank = card.currentPlayerCard.currentRarity;

        int splashEffect = 4;
        int poisonZoneEffect = 5;

        GameObject poisonSplash = GameManager.instance.effectPoolManager.Get(splashEffect);
        GameObject poisonZoneObj = GameManager.instance.effectPoolManager.Get(poisonZoneEffect);

        // ✅ 변경: Bullet_ZonePoison 사용
        Bullet_ZonePoison poisonZone = poisonZoneObj.GetComponent<Bullet_ZonePoison>();

        // 드랍 포인트
        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(eventData.position.x, eventData.position.y, Camera.main.nearClipPlane)
        );
        targetPosition.z = 0;

        poisonSplash.transform.position = targetPosition;
        poisonZoneObj.transform.position = targetPosition;

        // 랭크별 스탯
        float magicDamage = GetDamageByRank(rank);              // "초당 100%"의 기준 데미지 (dps)
        float zoneDuration = GetZoneDurationByRank(rank);       // 장판 유지 시간
        float poisonDuration = GetPoisonDurationByRank(rank);   // ✅ 중독 지속 시간

        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(
            magicDamage, out bool isCritical
        );

        // ✅ poison은 크리 안 쓰는 정책이면 isCritical은 무시해도 됨
        // ✅ 변경: (dps, zoneDuration, poisonDuration)
        poisonZone.Init(finalDamage, zoneDuration, poisonDuration);
    }

    private float GetDamageByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 1f;
            case RankType.Rare:      return 1.5f;
            case RankType.Epic:      return 2f;
            case RankType.Legendary: return 2.5f;
            default:                return 1f;
        }
    }

    // ✅ 기존 duration은 "장판 지속시간"으로 이름 바꿔주는게 명확함
    private float GetZoneDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 3f;
            case RankType.Rare:      return 4f;
            case RankType.Epic:      return 5f;
            case RankType.Legendary: return 6f;
            default:                return 3f;
        }
    }

    // ✅ 중독 지속시간은 따로 튜닝 가능
    private float GetPoisonDurationByRank(RankType rank)
    {
        switch (rank)
        {
            case RankType.Uncommon:  return 10f;
            case RankType.Rare:      return 10f;
            case RankType.Epic:      return 10f;
            case RankType.Legendary: return 10f;
            default:                return 10f;
        }
    }
}
