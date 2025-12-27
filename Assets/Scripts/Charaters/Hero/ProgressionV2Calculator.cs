using System;
using System.Collections.Generic;
using UnityEngine;
using Game.RankSystem; // RankType

[Serializable]
public struct StatBlock
{
    public float hp;
    public float moveSpeed;
    public float critChance;
    public float critDamage;
    public int attack;
}

public class ProgressionV2Calculator
{
    private readonly HeroScriptableObject data;

    private readonly Dictionary<(RankType rank, int level), StatBlock> statCache = new();
    private readonly Dictionary<RankType, List<string>> skillCache = new();

    public ProgressionV2Calculator(HeroScriptableObject data)
    {
        this.data = data;
    }

    public StatBlock GetStats(RankType rank, int level)
    {
        level = Mathf.Clamp(level, 1, data.maxLevel);
        var key = (rank, level);

        if (statCache.TryGetValue(key, out var cached))
            return cached;

        var s = new StatBlock
        {
            hp = data.hp,
            moveSpeed = data.moveSpeed,
            critChance = data.critChance,
            critDamage = data.critDamage,
            attack = data.baseAttack
        };

        // ✅ 매 레벨업 공격력 증가
        s.attack += (level - 1) * data.attackPerLevel;

        // ✅ 퍼센트 합산(+10% +10% = +20%)
        float hpPct = 0f, atkPct = 0f, spdPct = 0f, ccPct = 0f, cdPct = 0f;

        // ✅ Lv 3/6/9/12/15 마일스톤 적용
        foreach (var ms in data.levelMilestones)
        {
            if (ms.level > level) continue;

            foreach (var mod in ms.mods)
            {
                switch (mod.stat)
                {
                    case StatType.HP:
                        if (mod.op == ModOp.Add) s.hp += mod.value;
                        else hpPct += mod.value;
                        break;

                    case StatType.MoveSpeed:
                        if (mod.op == ModOp.Add) s.moveSpeed += mod.value;
                        else spdPct += mod.value;
                        break;

                    case StatType.Attack:
                        if (mod.op == ModOp.Add) s.attack += Mathf.RoundToInt(mod.value);
                        else atkPct += mod.value;
                        break;

                    case StatType.CritChance:
                        if (mod.op == ModOp.Add) s.critChance += mod.value; // +0.10 = 10%p
                        else ccPct += mod.value;
                        break;

                    case StatType.CritDamage:
                        if (mod.op == ModOp.Add) s.critDamage += mod.value;
                        else cdPct += mod.value;
                        break;
                }
            }
        }

        // ✅ 합산 퍼센트 최종 반영
        s.hp *= (1f + hpPct);
        s.moveSpeed *= (1f + spdPct);
        s.attack = Mathf.RoundToInt(s.attack * (1f + atkPct));
        s.critChance = Mathf.Clamp01(s.critChance * (1f + ccPct));
        s.critDamage *= (1f + cdPct);

        statCache[key] = s;
        return s;
    }

    public IReadOnlyList<string> GetUnlockedSkills(RankType currentRank)
    {
        if (skillCache.TryGetValue(currentRank, out var cached))
            return cached;

        var list = new List<string>();

        // ✅ 현재 랭크 이하 해금 전부 누적
        foreach (var ru in data.rankUnlocks)
        {
            if (ru.rank <= currentRank)
                list.AddRange(ru.skillIds);
        }

        skillCache[currentRank] = list;
        return list;
    }
}
