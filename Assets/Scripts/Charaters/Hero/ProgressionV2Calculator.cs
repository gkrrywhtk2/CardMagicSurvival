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
        int MAXLEVEL = HeroManager.MAX_LEVEL;
        level = Mathf.Clamp(level, 1, MAXLEVEL);
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

        // ✅ 퍼센트 합산(상대 증가용) - 치확/치피는 제외
        float hpPct = 0f, atkPct = 0f, spdPct = 0f;

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
                        if (mod.op == ModOp.Add)
                            s.critChance += mod.value * 0.01f; // 10 => +0.10 (10%p)
                        break;

                    case StatType.CritDamage:
                        if (mod.op == ModOp.Add)
                            s.critDamage += mod.value * 0.01f; // 10 => +0.10 (150% -> 160% if base=1.5)
                        break;
                }
            }
        }

        // ✅ 상대 증가 스탯만 곱 반영
        s.hp *= (1f + hpPct);
        s.moveSpeed *= (1f + spdPct);
        s.attack = Mathf.RoundToInt(s.attack * (1f + atkPct));

        // ✅ 치확/치피는 가산이므로 Clamp/방어만
        s.critChance = Mathf.Clamp01(s.critChance);
        s.critDamage = Mathf.Max(0f, s.critDamage);


        statCache[key] = s;
        return s;
    }

    // public IReadOnlyList<string> GetUnlockedSkills(RankType currentRank)
    // {
    //     if (skillCache.TryGetValue(currentRank, out var cached))
    //         return cached;

    //     var list = new List<string>();

    //     // ✅ 현재 랭크 이하 해금 전부 누적
    //     foreach (var ru in data.rankUnlocks)
    //     {
    //         // if (ru.rank <= currentRank)
    //         //     list.AddRange(ru.skillIds);
    //     }

    //     skillCache[currentRank] = list;
    //     return list;
    // }
}
