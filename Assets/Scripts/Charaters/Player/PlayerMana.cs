using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMana : MonoBehaviour
{
   // =======================
// Mana Recovery System
// =======================

[Header("#Mana")]
public float maxMana = 9f;

// "순수 기본값" (캐릭터 고유/레벨업/기초 세팅)
public float baseManaRecoveryRaw = 0.5f;

// 기본 회복량에만 적용되는 "기본 증가 옵션" (장비/아이템 등)
private float baseManaRecoveryFlatBonus = 0f;     // 기본 회복량 +0.2 같은 것(선택)
private float baseManaRecoveryPercentBonus = 0f;  // 기본 회복량 +10% (=0.10f)

// 추가 회복량(카드/버프/일시효과 등): 전부 중첩 + 각각 만료
private int _nextManaBonusId = 1;

private class ManaBonus
{
    public int id;
    public float flat;        // 추가 회복량(+1 같은 것)
    public Coroutine expire;  // 만료 코루틴
}

private readonly Dictionary<int, ManaBonus> manaBonusMap = new Dictionary<int, ManaBonus>();
private float totalBonusFlatManaRecovery = 0f; // 추가 회복량 합(캐시)

// 내부 변수 + clamp
public float _mana;
public float mana
{
    get => _mana;
    set => _mana = Mathf.Clamp(value, 0f, maxMana);
}

  // 매 프레임 회복 (Update에서 호출)
    void Update()
    {
        ManaRecovery();
    }
    public void ManaRecovery()
    {
        if (!GameManager.instance.GamePlayState) return;

        // 기본 회복량(퍼센트는 기본에만!)
        float effectiveBase = (baseManaRecoveryRaw + baseManaRecoveryFlatBonus) * (1f + baseManaRecoveryPercentBonus);

        // 추가 회복량은 퍼센트 영향 X (밸런스 규칙)
        float total = effectiveBase + totalBonusFlatManaRecovery;

        mana += total * Time.deltaTime;
    }

// -----------------------
// Base Recovery modifiers (아이템/장비)
// -----------------------

// 기본 회복량 +% (추가 회복량에는 영향 없음)
public void AddBaseManaRecoveryPercent(float percent)   // ex) 0.10f = 10%
{
    baseManaRecoveryPercentBonus += percent;
}
public void RemoveBaseManaRecoveryPercent(float percent)
{
    baseManaRecoveryPercentBonus -= percent;
}

// 기본 회복량 +고정값 (선택: 필요 없으면 안 써도 됨)
public void AddBaseManaRecoveryFlat(float flat)
{
    baseManaRecoveryFlatBonus += flat;
}
public void RemoveBaseManaRecoveryFlat(float flat)
{
    baseManaRecoveryFlatBonus -= flat;
}

// -----------------------
// Bonus Recovery (카드/버프) : 전부 중첩 + 시간제
// -----------------------

public int AddBonusManaRecoveryFlatTimed(float flat, float duration, bool useUnscaledTime = false)
{
    int id = _nextManaBonusId++;

    var bonus = new ManaBonus { id = id, flat = flat, expire = null };
    manaBonusMap[id] = bonus;

    // 합 캐시 갱신
    totalBonusFlatManaRecovery += flat;

    // 만료 예약
    bonus.expire = StartCoroutine(ExpireBonusMana(id, duration, useUnscaledTime));
    return id;
}

private IEnumerator ExpireBonusMana(int id, float duration, bool useUnscaledTime)
{
    if (useUnscaledTime) yield return new WaitForSecondsRealtime(duration);
    else yield return new WaitForSeconds(duration);

    RemoveBonusManaRecoveryById(id);
}

public void RemoveBonusManaRecoveryById(int id)
{
    if (!manaBonusMap.TryGetValue(id, out var bonus)) return;

    if (bonus.expire != null)
        StopCoroutine(bonus.expire);

    // 합 캐시 갱신
    totalBonusFlatManaRecovery -= bonus.flat;

    manaBonusMap.Remove(id);
}

}
