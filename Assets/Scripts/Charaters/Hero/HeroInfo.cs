using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroInfo : MonoBehaviour
{
    [Header("Refs")]
    public TMP_Text name_text;
    public TMP_Text lv_text;
    public RankLabelUI rankLabel;
    public Slider exp_slider;
    public TMP_Text slider_text;
    public TMP_Text atk_text;
    public TMP_Text hp_text;
    public TMP_Text moveSpeed_text;
    public TMP_Text criChance_text;
    public TMP_Text criPer_text;   
    public GameObject isSlectedButton;//선택중 버튼
    public GameObject isSelectButton;//선택 버튼

    public LevelUpStatUI[] levelUpStats;

    [Header("Data")]
    public int id;
    public int level;
    public int exp;
    public RankType rank;
    public bool isSelected;

    public void Init(int heroId, int heroLevel, int heroExp, RankType heroRank, bool isSelected)
    {
        id = heroId;
        level = heroLevel;
        exp = heroExp;
        rank = heroRank;
        this.isSelected = isSelected;
        IsSelectedButtonSetting(isSelected);
        AnimationSetup(id);
    }

    public void RefreshUI()
    {
        var heroSO = HeroManager.Instance.GetHeroSO(id);
        if (heroSO == null)
        {
            Debug.LogError($"[HeroInfo] Hero SO not found for id={id}", this);
            return;
        }

        UpdateBasicInfo(heroSO);
        UpdateStats(heroSO);
        UpdateExpSlider();
        rankLabel.SetRank(rank);

        // ✅ 마일스톤 UI 갱신
        RefreshMilestonesFixed(heroSO);
    }

    public void IsSelectedButtonSetting(bool selected)
    {
        if (selected)
        {
            isSlectedButton.SetActive(true);
            isSelectButton.SetActive(false);
        }
        else
        {
            isSlectedButton.SetActive(false);
            isSelectButton.SetActive(true);
        }
    }

    private void UpdateBasicInfo(HeroScriptableObject heroSO)
    {
        name_text.text = heroSO.nameKor;
        lv_text.text = $"Lv. {level}";
    }

    private void UpdateStats(HeroScriptableObject heroSO)
    {
        var calc = new ProgressionV2Calculator(heroSO);
        var stats = calc.GetStats(rank, level);

        atk_text.text = stats.attack.ToString();
        hp_text.text = stats.hp.ToString("F0");
        moveSpeed_text.text = stats.moveSpeed.ToString("F1");

        // 0.1 -> 10%
        criChance_text.text = (stats.critChance * 100f).ToString("F1") + "%";

        // 2.0 -> 200% (2배로 인식)
        criPer_text.text = (stats.critDamage * 100f).ToString("F0") + "%";
    }

    private void UpdateExpSlider()
    {
        int expForNextLevel = HeroManager.Instance.MaxExpSetting(level);
        exp_slider.maxValue = expForNextLevel;
        exp_slider.value = Mathf.Min(exp, expForNextLevel);
        slider_text.text = $"{exp_slider.value}/{exp_slider.maxValue}";
    }
        private List<StatMod> GetActiveMilestoneMods(HeroScriptableObject heroSO, int currentLevel)
    {
        var result = new List<StatMod>();
        if (heroSO.levelMilestones == null) return result;

        foreach (var ms in heroSO.levelMilestones)
        {
            if (ms == null || ms.mods == null) continue;
            if (ms.level > currentLevel) continue;

            result.AddRange(ms.mods);
        }

        return result;
    }

    [Header("Per Hero Run Clips (UI Image용)")]
    public Animator animator;

    // 캐싱(히어로 바꿀 때 매번 새로 생성 안 하게)

    public RuntimeAnimatorController[] animatorController;

    public void AnimationSetup(int heroId)
    {
        if (animator == null) { Debug.LogError("Animator 없음"); return; }
        animator.runtimeAnimatorController = animatorController[heroId];
    }


     [Header("Mailstone UI")]

    [Header("Milestone UI (Fixed Slots: 3/6/9/12/15)")]
    [SerializeField] private MilestoneRowUI[] milestoneRows; // size=5, [0]=Lv3 ...
public TMP_Text tmp;
            private void RefreshMilestonesFixed(HeroScriptableObject heroSO)
    {
        if (milestoneRows == null || milestoneRows.Length < 5)
        {
            Debug.LogError("[HeroInfo] milestoneRows 배열(5개) 할당 필요", this);
            return;
        }

        // 1) 기본 초기화: 5개 슬롯 모두 표시 + 잠김 아이콘 ON
        for (int i = 0; i < milestoneRows.Length; i++)
        {
            var row = milestoneRows[i];
            if (row == null) continue;

            row.gameObject.SetActive(true);
            row.SetLocked(true);
        }

        if (heroSO == null || heroSO.levelMilestones == null) return;

        // 2) 각 milestone을 고정 슬롯에 채우기
        foreach (var ms in heroSO.levelMilestones)
        {
            if (ms == null || ms.mods == null || ms.mods.Count == 0) continue;

            int idx = MilestoneIndex(ms.level);
            if (idx < 0 || idx >= milestoneRows.Length) continue;

            var row = milestoneRows[idx];
            if (row == null) continue;

            var mod = ms.mods[0]; // 1개 고정
            if (mod == null) continue;

            // 문구는 항상 세팅(잠김이어도 “무슨 효과인지” 보여주려면)
            row.Bind(mod);

            // ✅ 잠김 여부에 따라 자물쇠 아이콘 토글
            bool unlocked = level >= ms.level;
            row.SetLocked(!unlocked);


                var t = tmp; // TMP_Text
    Debug.Log($"Base Font = {t.font?.name}");
    if (t.font != null && t.font.fallbackFontAssetTable != null)
    {
        Debug.Log("Fallbacks:");
        foreach (var f in t.font.fallbackFontAssetTable)
            Debug.Log($" - {f?.name}");
    }

        }
    }

    


        private int MilestoneIndex(int lv) => lv switch
    {
        3  => 0,
        6  => 1,
        9  => 2,
        12 => 3,
        15 => 4,
        _  => -1
    };
}

[System.Serializable]
public class RankLabelUI
{
    public Image background;
    public Image inner;
    public TMP_Text text;

    public void SetRank(RankType rank)
    {
        var (rankName, frameColor, innerColor) = GetRankColors(rank);
        
        text.text = rankName;
        background.color = frameColor;
        inner.color = innerColor;
    }

    private (string name, Color32 frame, Color32 inner) GetRankColors(RankType rank)
    {
        return rank switch
        {
            RankType.Uncommon => (
                "Uncommon",
                new Color32(0x8A, 0x8D, 0x93, 0xFF),
                new Color32(0xA5, 0xA7, 0xAB, 0xFF)
            ),
            RankType.Rare => (
                "Rare",
                new Color32(0x50, 0x9A, 0xFF, 0xFF),
                new Color32(0x85, 0xBC, 0xFF, 0xFF)
            ),
            RankType.Epic => (
                "Epic",
                new Color32(0xB3, 0x63, 0xDF, 0xFF),
                new Color32(0xCC, 0x96, 0xEA, 0xFF)
            ),
            RankType.Legendary => (
                "Legendary",
                new Color32(0xEB, 0xB6, 0x2D, 0xFF),
                new Color32(0xF2, 0xCE, 0x72, 0xFF)
            ),
            RankType.Mythic => (
                "Mythic",
                new Color32(0xFF, 0x5C, 0x7C, 0xFF), // frameColor 추가 필요
                new Color32(0xE1, 0x80, 0x9A, 0xFF)
            ),
            _ => (
                "Unknown",
                Color.white,
                Color.white
            )
        };
    }
}

[System.Serializable]
public class LevelUpStatUI
{
    public TMP_Text text;
    public bool isBold;

    public void SetValue(string value, bool highlight = false)
    {
        text.text = value;
        text.fontStyle = (highlight || isBold) ? FontStyles.Bold : FontStyles.Normal;
    }
}