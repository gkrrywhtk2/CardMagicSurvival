using UnityEngine;
using UnityEngine.EventSystems;

public class Card5_FireBall : MonoBehaviour, ICardUse
{
    // ✅ 고정 크기
    private static readonly Vector3 FixedScale = new Vector3(2.5f, 2.5f, 1f);

    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null) return;

        var data = card.cardScritableData;

        // ✅ 등급 대신 레벨 사용
        int level = card.currentPlayerCard.LEVEL;

        int fireBallNum = 7;
        Bullet_2 fireBall = GameManager.instance.objectPooling.Get(fireBallNum).GetComponent<Bullet_2>();

        Vector2 targetPosition = Camera.main.ScreenToWorldPoint(new Vector2(eventData.position.x, eventData.position.y));
        Vector2 playerVec = GameManager.instance.player.playerCenterPivot.position;

        // 생성 시 플레이어 위치
        fireBall.transform.position = playerVec;

        Vector2 direction = (targetPosition - playerVec).normalized;
        float bulletSpeed = 7f;
        int per = -1; // 무한

        // ✅ 레벨 기반 데미지
        float damage = data.GetDamage(level);

        // ✅ 크기는 고정
        fireBall.ScaleSetting(FixedScale);

        float finalDamage = GameManager.instance.player.playerStatus.DamageReturn(damage, out bool isCritical);
        fireBall.Init(direction, bulletSpeed, per, finalDamage, isCritical);
    }

}
