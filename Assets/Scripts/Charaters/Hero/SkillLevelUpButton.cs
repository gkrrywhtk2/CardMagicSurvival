using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SkillLevelUpButton : MonoBehaviour
{
    private const int MAX_LEVEL = 10;

    [Header("UI")]
    [SerializeField] private LocalizeStringEvent levelUpLocalize; // ✅ 버튼 라벨 로컬라이즈
    public TMP_Text valueText;
    public Image lockBg;
    public Image valueBg;

    [Header("Localization Keys")]
    [SerializeField] private string table = "UI_Common";
    [SerializeField] private string keyLevelUp = "Skill.LevelUp";
    [SerializeField] private string keyMaxLevel = "Skill.MaxLevel";

    [Header("Refs")]
    public SkillPanel skillPanel;
    public HeroInfo heroInfo;

    private Button btn;
    private int value;
    private int level;

    private void Awake()
    {
        btn = GetComponent<Button>();
        if (levelUpLocalize == null) levelUpLocalize = GetComponentInChildren<LocalizeStringEvent>(true);
    }

    public void Init(int value, int level)
    {
        this.value = value;
        this.level = level;

        if (ServerDataManager.instance == null)
        {
            Debug.LogWarning("[SkillLevelUpButton] ServerDataManager.instance is null");
            SetInteractable(false);
            return;
        }

        int currentUpgradeStone = ServerDataManager.instance.GetCurrentUpgradeStone();
        bool isMax = level >= MAX_LEVEL;
        bool canPay = currentUpgradeStone >= value;
        bool canUpgrade = !isMax && canPay;

        // ✅ 로컬라이즈 라벨 변경
        SetLabelKey(isMax ? keyMaxLevel : keyLevelUp);

        valueBg.gameObject.SetActive(!isMax);
        valueText.gameObject.SetActive(!isMax);

        if (!isMax)
        {
            valueText.text = value.ToString();
            valueText.color = canPay ? Color.white : Color.red;
        }

        lockBg.gameObject.SetActive(!canUpgrade);
        SetInteractable(canUpgrade);
    }

    private void SetLabelKey(string key)
    {
        if (levelUpLocalize == null) return;
        levelUpLocalize.StringReference = new LocalizedString(table, key);
        levelUpLocalize.RefreshString();
    }

    public void UpgradeSkillLevel()
    {
        if (ServerDataManager.instance == null) return;
        if (skillPanel == null || heroInfo == null) return;

        var hero = skillPanel.data;
        var rank = skillPanel.rank;
        if (hero == null) return;

        if (level >= MAX_LEVEL) return;

        int nowStone = ServerDataManager.instance.GetCurrentUpgradeStone();
        if (nowStone < value) return;

        bool upgraded = ServerDataManager.instance.TryChangeSelectedHeroSkillLevel(hero, rank, +1, 0, MAX_LEVEL);
        if (!upgraded) return;

        ServerDataManager.instance.AddUpgradeStone(-value);

        skillPanel.RefreshUI();
        heroInfo.RefreshUI();
    }

    private void SetInteractable(bool on)
    {
        if (btn != null) btn.interactable = on;
    }
}
