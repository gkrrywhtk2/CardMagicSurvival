using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerCritical : MonoBehaviour
{
    [Header("Critical Chance")]
    [Range(0f, 100f)]
    public float baseCritChance = 0f;     // 기본 치확(0~100)

    [Header("Critical Damage")]
    [Min(1f)]
    public float critMultiplier = 0f;     // ✅ 치명타 배율 (2 = 2배)

    private float totalBonusCritChance = 0f;
    private int _nextId = 1;

    private class Bonus
    {
        public int id;
        public float value;
        public Coroutine co;
    }

    private readonly Dictionary<int, Bonus> bonuses = new();

    public float CurrentCritChance => Mathf.Clamp(baseCritChance + totalBonusCritChance, 0f, 100f);

    public bool RollCritical()
    {
        return Random.Range(0f, 100f) < CurrentCritChance;
    }

    public int AddBonusCritChanceTimed(float value, float duration, bool useUnscaledTime = false)
    {
        int id = _nextId++;
        var b = new Bonus { id = id, value = value, co = null };
        bonuses[id] = b;

        totalBonusCritChance += value;
        b.co = StartCoroutine(RemoveAfter(id, duration, useUnscaledTime));
        return id;
    }

    private IEnumerator RemoveAfter(int id, float duration, bool useUnscaledTime)
    {
        if (useUnscaledTime) yield return new WaitForSecondsRealtime(duration);
        else yield return new WaitForSeconds(duration);

        RemoveBonusCritChance(id);
    }

    public void RemoveBonusCritChance(int id)
    {
        if (!bonuses.TryGetValue(id, out var b)) return;

        if (b.co != null) StopCoroutine(b.co);

        totalBonusCritChance -= b.value;
        bonuses.Remove(id);
    }
}
