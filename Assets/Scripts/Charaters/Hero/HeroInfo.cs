using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public LevelUpStatUI[] levelUpStats;

    [Header("Data")]
    public int id;
    public int level;
    public int exp;
    public RankType rank;

    public void Init(int heroId, int heroLevel, int heroExp, RankType heroRank)
    {
        id = heroId;
        level = heroLevel;
        exp = heroExp;
        rank = heroRank;
    }

    public void Calculate()
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
        criChance_text.text = stats.critChance.ToString("F1") + "%";
        criPer_text.text = stats.critDamage.ToString("F1") + "%";
    }

    private void UpdateExpSlider()
    {
        int expForNextLevel = HeroManager.Instance.MaxExpSetting(level);
        exp_slider.maxValue = expForNextLevel;
        exp_slider.value = Mathf.Min(exp, expForNextLevel);
        slider_text.text = $"{exp_slider.value}/{exp_slider.maxValue}";
    }
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