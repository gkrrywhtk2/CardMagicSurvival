using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveSpeed : MonoBehaviour
{
    [Header("Base Speed")]
    public float baseSpeed = 5f;

    private int _nextId = 1;

    private class SpeedBonus
    {
        public int id;
        public float value;        // +이동속도 (flat)
        public Coroutine expire;   // 만료 코루틴(시간제일 때)
    }

    private readonly Dictionary<int, SpeedBonus> bonusMap = new();
    private float totalBonusSpeed = 0f; // 캐시

    // ✅ 최종 이동속도
    public float CurrentSpeed => Mathf.Max(0f, baseSpeed + totalBonusSpeed);

    // -----------------------
    // Bonus Speed (ID 기반)
    // -----------------------

    /// <summary>
    /// 영구(또는 수동 해제) 추가 이동속도. (장비/패시브)
    /// </summary>
    public int AddBonusSpeed(float value)
    {
        int id = _nextId++;

        var b = new SpeedBonus { id = id, value = value, expire = null };
        bonusMap[id] = b;

        totalBonusSpeed += value;
        return id;
    }

    /// <summary>
    /// 시간제 추가 이동속도. (카드/버프)
    /// </summary>
    public int AddBonusSpeedTimed(float value, float duration, bool useUnscaledTime = false)
    {
        int id = AddBonusSpeed(value);
        bonusMap[id].expire = StartCoroutine(Expire(id, duration, useUnscaledTime));
        return id;
    }

    private IEnumerator Expire(int id, float duration, bool useUnscaledTime)
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(duration);
        else yield return new WaitForSeconds(duration);

        RemoveBonusSpeedById(id);
    }

    /// <summary>
    /// ID로 개별 제거 (꼬임 방지)
    /// </summary>
    public void RemoveBonusSpeedById(int id)
    {
        if (!bonusMap.TryGetValue(id, out var b)) return;

        if (b.expire != null)
            StopCoroutine(b.expire);

        totalBonusSpeed -= b.value;
        bonusMap.Remove(id);
    }

    // (선택) 디버그용
    public float GetTotalBonusSpeed() => totalBonusSpeed;
}
