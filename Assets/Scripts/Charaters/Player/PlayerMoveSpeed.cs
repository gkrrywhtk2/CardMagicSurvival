using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveSpeed : MonoBehaviour
{
    [Header("Base Speed")]
    public float baseSpeed = 3f;

    private int _nextId = 1;
    
    // ✅ 이동 불가 상태 관리
    private bool isMovementLocked = false;

    private class SpeedBonus
    {
        public int id;
        public float value;
        public Coroutine expire;
    }

    private readonly Dictionary<int, SpeedBonus> bonusMap = new();
    private float totalBonusSpeed = 0f;

    // ✅ 이동 잠금 시 0 반환
    public float CurrentSpeed => isMovementLocked ? 0f : Mathf.Max(0f, baseSpeed + totalBonusSpeed);

    // -----------------------
    // Movement Lock
    // -----------------------

    /// <summary>
    /// 이동 잠금 (영구)
    /// </summary>
    public void LockMovement()
    {
        isMovementLocked = true;
    }

    /// <summary>
    /// 이동 잠금 해제
    /// </summary>
    public void UnlockMovement()
    {
        isMovementLocked = false;
    }

    /// <summary>
    /// 시간제 이동 잠금 (아티팩트 이벤트 등)
    /// </summary>
    public void LockMovementForDuration(float duration)
    {
        StartCoroutine(LockMovementCoroutine(duration));
    }

    private IEnumerator LockMovementCoroutine(float duration)
    {
        isMovementLocked = true;
        yield return new WaitForSeconds(duration);
        isMovementLocked = false;
    }

    // -----------------------
    // Bonus Speed (기존 코드)
    // -----------------------

    public int AddBonusSpeed(float value)
    {
        int id = _nextId++;
        var b = new SpeedBonus { id = id, value = value, expire = null };
        bonusMap[id] = b;
        totalBonusSpeed += value;
        return id;
    }

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

    public void RemoveBonusSpeedById(int id)
    {
        if (!bonusMap.TryGetValue(id, out var b)) return;
        if (b.expire != null) StopCoroutine(b.expire);
        totalBonusSpeed -= b.value;
        bonusMap.Remove(id);
    }

    public float GetTotalBonusSpeed() => totalBonusSpeed;
}