using System;
using System.Collections.Generic;
using UnityEngine;
using Game.RankSystem;

public enum StatType { HP, MoveSpeed, Attack, CritChance, CritDamage }
public enum ModOp { Add, PercentAdd } // PercentAdd: +0.10 (= +10%)

[Serializable]
public class StatMod
{
    public StatType stat;
    public ModOp op;
    public float value; // Add: +10, PercentAdd: +0.10
}

[Serializable]
public class LevelMilestone
{
    public int level; // 3,6,9,12,15
    public List<StatMod> mods = new();
}

[Serializable]
public class RankUnlock
{
    public RankType rank;     
    public List<string> skillIds = new();
}

[CreateAssetMenu(menuName = "Game/Progression/Hero Scriptable Object")]
public class HeroScriptableObject : ScriptableObject
{

    [Header("Identity")]
    public int heroId;

    [Header("Visual")]
    public RuntimeAnimatorController animatorController;

    [Header("Base Stats")]
    public float hp = 90;
    public float moveSpeed = 3f;
    public float critChance = 0.10f;  // 10% = 0.10
    public float critDamage = 2.0f;   // 200% = 2.0 (배수)
    public int baseAttack = 15;

    [Header("Level Settings")]
    public int maxLevel = 15;

    [Tooltip("레벨업마다 항상 증가하는 공격력(예: +2)")]
    public int attackPerLevel = 2;

    [Tooltip("Lv 3/6/9/12/15 에서만 추가 적용되는 마일스톤")]
    public List<LevelMilestone> levelMilestones = new();

    [Header("Rank Unlocks (Skills)")]
    public List<RankUnlock> rankUnlocks = new();

    private static readonly HashSet<int> AllowedMilestones = new() { 3, 6, 9, 12, 15 };

    
}
