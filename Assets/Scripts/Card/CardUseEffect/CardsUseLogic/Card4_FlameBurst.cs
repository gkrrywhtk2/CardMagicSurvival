using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;
using Game.CardData;

public class Card4_FlameBurst : MonoBehaviour, ICardUse
{
    [Header("# Data Reference")]
    public CardScritableData data; // 인스펙터에서 해당 SO를 꼭 할당해주세요!

    private Coroutine flameBurstCorutine;
    private static readonly Vector3 FixedScale = new Vector3(2f, 2f, 1f);

    public void Use(PointerEventData eventData)
    {
        if (eventData == null || eventData.pointerDrag == null) return;

        MagicCard card = eventData.pointerDrag.GetComponent<MagicCard>();
        if (card == null || card.currentPlayerCard == null) return;

        data = card.cardScritableData;
        // 카드에서 현재 레벨(Stack)을 가져옴
        int level = card.currentPlayerCard.LEVEL;

        // 기존 Coroutine이 실행 중이면 중단
        if (flameBurstCorutine != null)
            StopCoroutine(flameBurstCorutine);

        // 새로운 Coroutine 시작
        flameBurstCorutine = StartCoroutine(FlameBurstRoutine(level));
    }

    private IEnumerator FlameBurstRoutine(int level)
    {

       // data = CardData.Instance.cardScritableData[4];
        // ✅ SO에서 데이터 가져오기
        float damage = data.GetDamage(level);
        int repeatCount = data.GetCount(level); // SO의 baseCount와 growth 활용
        int flameburstObjectNum = 7; 

        for (int i = 0; i < repeatCount; i++)
        {
            // 데미지 세팅 (플레이어 스탯 적용)
            float finalDamage = GameManager.instance.player.playerStatus
                .DamageReturn(damage, out bool isCritical);

            // FlameBurst 효과 생성 및 초기화
            GameObject flame = GameManager.instance.effectPoolManager.Get(flameburstObjectNum);
            flame.GetComponent<Melee>().Init(finalDamage, isCritical);
            flame.GetComponent<Melee>().ScaleSetting(FixedScale);

            // 위치 및 회전 설정
            Vector2 skillPosition = GameManager.instance.player.dirFront.skillPosition;
            float angle = GameManager.instance.player.dirFront.angle;

            flame.transform.position = skillPosition;
            flame.transform.rotation = Quaternion.Euler(0, 0, angle);

            // 반복 간격
            yield return new WaitForSeconds(0.25f);
        }

        flameBurstCorutine = null;
    }

    /// <summary>
    /// UI 등에서 설명을 출력할 때 호출하는 메서드
    /// </summary>
    public string GetLocalizedDescription(int level)
    {
        if (data == null) return "Data Missing";
        return data.GetParsedDescription(level);
    }
}