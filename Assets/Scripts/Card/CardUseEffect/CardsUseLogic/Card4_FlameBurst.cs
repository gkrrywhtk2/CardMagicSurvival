using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Card4_FlameBurst : MonoBehaviour, ICardUse
{
    public Coroutine flameBurstCorutine;

    // ✅ 고정값
    private const int RepeatCount = 6;
    private static readonly Vector3 FixedScale = new Vector3(2f, 2f, 1f);

    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null) return;

        // ✅ 등급 대신 레벨 사용
        int level = card.currentPlayerCard.LEVEL;

        // 기존 Coroutine이 실행 중이면 중단
        if (flameBurstCorutine != null)
            StopCoroutine(flameBurstCorutine);

        // 새로운 Coroutine 시작
        flameBurstCorutine = StartCoroutine(FlameBurstRoutine(level));
    }

    private IEnumerator FlameBurstRoutine(int level)
    {
        float damage = GetDamageByLevel(level);
        int flameburstObjectNum = 7; // 오브젝트 풀에서 가져올 ID

        // ✅ 6회 고정
        for (int i = 0; i < RepeatCount; i++)
        {
            // 데미지 세팅
            float finalDamage = GameManager.instance.player.playerStatus
                .DamageReturn(damage, out bool isCritical);

            // FlameBurst 효과 생성
            GameObject flame = GameManager.instance.effectPoolManager.Get(flameburstObjectNum);
            flame.GetComponent<Melee>().Init(finalDamage, isCritical);
            flame.GetComponent<Melee>().ScaleSetting(FixedScale);

            // 플레이어의 바로 앞 스킬이 연출될 좌표
            Vector2 skillPosition = GameManager.instance.player.dirFront.skillPosition;
            float angle = GameManager.instance.player.dirFront.angle;

            flame.transform.position = skillPosition;
            flame.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 0.25초 마다 반복(기존 유지)
            yield return new WaitForSeconds(0.25f);
        }

        flameBurstCorutine = null;
    }

    /// <summary>
    /// ✅ 레벨 기반 데미지 공식
    /// - Lv1  : 5
    /// - Lv25 : 8
    /// 선형 증가: 5 + (L-1) * (3/24) = 5 + (L-1)*0.125
    /// </summary>
    private float GetDamageByLevel(int level)
    {
        level = Mathf.Clamp(level, 1, 99);
        return 5f + (level - 1) * 0.125f; // Lv25 -> 8
    }
}
