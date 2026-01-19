using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeroInfo : MonoBehaviour
{
    [Header("Localization")]
    public HeroNameUI heroNameUI;
    public RankLocalization rankLocalization;
    [Header("Refs")]
    public TMP_Text name_text;
    public TMP_Text lv_text;
    public RankLabelUI rankLabel;
    public Slider exp_slider;
        public Color blue_Color;
        public Color green_Color;
        public Image exp_Slider_Fill;
        public GameObject UpArrow_Exp;
        public TMP_Text slider_text;
    public Slider upgrade_slider;
        public Image upgrade_Slider_Fill;
        public GameObject UpArrow_Upgrade;
        public TMP_Text upgrade_slider_text;
    public GameObject isSlectedButton;//선택중 버튼
    public GameObject isSelectButton;//선택 버튼

    public TMP_Text requireGold;//경험치 증가 필요 골드량 텍스트
    public RankUpButton rankUpButton;

    public LevelUpStatUI[] levelUpStats;
    public StatBg[] statBg;//스탯 블럭들 0 공격력, 1 체력, 2 이동속도 3  치명타확률 , 4 치명타 배율
    public HeroSkillFrame[] heroSkillFrames;
    public SkillPanel skillPanel;
    public HeroLevelUpButton heroLevelUpButton;
    

    [Header("Data")]
    public int id;
    public int level;
    public int exp;
    public RankType rank;
    public bool isSelected;
    public int UpgradeStone;
    public RankType nowTouchSkillFrame;//현재 터치된 스킬 프레임을 캐싱 -> 리프레싱UI할때 터치된 등급의 스킬을 계속 표시

    [Header("PanelControl")]
    public GameObject HeadView;
    public GameObject MiddleView_Level;
    public GameObject MiddleView_Rank;
    public GameObject[] lines;
    public GameObject BottomView_Level;
    public GameObject BottomView_Rank;
    public RectTransform MileStoneScroll;
    public Image LevelInfoButton;
    public Image RankInfoButton;
        public Color Pupple_Color;
        public Color DarkPupple_Color;
    public UpgradeStoneLabel upgradeStoneLabel;

        public void Init(int heroId, int heroLevel, int heroExp, RankType heroRank, bool isSelected)
    {
        id = heroId;
        level = heroLevel;
        exp = heroExp;
        rank = heroRank;
        this.isSelected = isSelected;

        IsSelectedButtonSetting(isSelected);
        AnimationSetup(id);
        heroNameUI.BindHeroName(id);
        rankLocalization.BindRank(rank);
        nowTouchSkillFrame = RankType.Uncommon;

        // ✅ root 기준으로 최종 확정
        RefreshUI();
    }

        public void RefreshUI()
    {
        // ✅ 항상 root(단일 상태)에서 최신 값으로 덮어쓰기
        var heroAccount = ServerDataManager.instance.GetHeroAccount(id);
        var upgradeStoneCount = ServerDataManager.instance.GetCurrentUpgradeStone();
        if (heroAccount == null)
        {
            Debug.LogWarning($"[HeroInfo] HeroAccount not found for id={id}");
            return;
        }

        level = heroAccount.level;
        exp = heroAccount.exp;
        rank = (RankType)heroAccount.rank;
        isSelected = heroAccount.isSelected;
        UpgradeStone = upgradeStoneCount;

        var heroSO = HeroManager.Instance.GetHeroSO(id);
        if (heroSO == null)
        {
            Debug.LogError($"[HeroInfo] Hero SO not found for id={id}", this);
            return;
        }

        UpdateBasicInfo(heroSO);

        bool levelupReady = UpdateExpSlider();
        UpdateUpgradeExpSlider();
        UpdateStats(heroSO, levelupReady);
        rankLabel.SetRank(rank);

        RefreshMilestonesFixed(heroSO);

        SelectButtonSetting();
        UpdateRequireGoldColor();

        SettingHeroSkillFrame(id);
        RefreshSkillPanel(nowTouchSkillFrame);

        // 랭크업 버튼 세팅
        int RequireUpgradeStone = HeroManager.Instance.MaxUpgradeExpSetting(rank);
        rankUpButton.Init(RequireUpgradeStone);

        //랭크 라벨 세팅
        rankLocalization.BindRank(rank);
        heroLevelUpButton.Init(heroAccount);
    }
    public void RefreshSkillPanel(RankType rank)
    {
        var heroAccount = ServerDataManager.instance.GetHeroAccount(id);
        var upgradeStoneCount = ServerDataManager.instance.GetCurrentUpgradeStone();
        skillPanel.Init(heroAccount,rank);
    }

    private void UpdateRequireGoldColor()
    {
        int currentGold = ServerDataManager.instance.GetCurrentGold();
        int EXP_COST = 3000;
        
        if (currentGold >= EXP_COST)
        {
            requireGold.color = Color.white;
        }
        else
        {
            requireGold.color = Color.red;
        }
    }

    public void SettingHeroSkillFrame(int heroId)
    {
        var sdm = ServerDataManager.instance;
        if (sdm == null) return;

        var hero = sdm.GetHeroAccount(heroId);
        if (hero == null)
        {
            Debug.LogWarning($"[HeroInfo] heroId={heroId} not found.");
            return;
        }

        // ✅ 종자 판정: mSkillLevel로 M 슬롯 존재 여부 판단
        bool canHaveMythicSkill = hero.mSkillLevel >= 1;

        for (int i = 0; i < heroSkillFrames.Length; i++)
        {
            var frame = heroSkillFrames[i];
            if (frame == null) continue;

            bool isMythicFrame = frame.slotType == SkillSlotType.M;

            if (isMythicFrame)
            {
                frame.gameObject.SetActive(canHaveMythicSkill);
                if (!canHaveMythicSkill) continue;
            }
            else
            {
                frame.gameObject.SetActive(true);
            }

            frame.Init(hero); // ✅ hero는 root의 HeroAccount
        }
    }



    public void SelectButtonSetting()
    {
        isSlectedButton.SetActive(isSelected);
        isSelectButton.SetActive(!isSelected);
    }
    public void SelectButton()
    {
        ServerDataManager.instance.UpdateSelectedHero(this.id);

        // 카드 리스트 갱신
        GameManager.instance.herocardManager.UpdateAllHeroCardFrames();

        // ✅ 이벤트 안 쓰니까: 내 패널도 직접 갱신
        RefreshUI();
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
        lv_text.text = $"Lv. {level}";
    }

    private void UpdateStats(HeroScriptableObject heroSO, bool LvUpready)
    {
        var calc = new ProgressionV2Calculator(heroSO);
        var stats = calc.GetStats(rank, level);
        var nextlevelStats= calc.GetStats(rank, level + 1);

        int atkValue = stats.attack;
        float hpValue = stats.hp;
        float moveSpeedValue = stats.moveSpeed;
        float critChanceValue = stats.critChance;
        float critDamageValue = stats.critDamage;

        int atkValue_next = nextlevelStats.attack;
        float hpValue_next = nextlevelStats.hp;
        float moveSpeedValue_next = nextlevelStats.moveSpeed;
        float critChanceValue_next = nextlevelStats.critChance;
        float critDamageValue_next = nextlevelStats.critDamage;

        
        statBg[0].Init(atkValue, atkValue_next,LvUpready);
        statBg[1].Init(hpValue, hpValue_next,LvUpready);
        statBg[2].Init(moveSpeedValue, moveSpeedValue_next,LvUpready);
        statBg[3].Init(critChanceValue,critChanceValue_next,LvUpready);
        statBg[4].Init(critDamageValue,critDamageValue_next,LvUpready);
    }

    private bool UpdateExpSlider()
    {
        int nowlevel = ServerDataManager.instance.GetCurrentLevel(id);

        // ✅ MAX 레벨이면 슬라이더/텍스트/화살표를 여기서 확정하고 끝
        if (nowlevel >= HeroManager.MAX_LEVEL)
        {
            exp_slider.maxValue = 1f;           // 0 방지용 (원하면 유지해도 됨)
            exp_slider.value = 1f;
            slider_text.text = "MAX";
            exp_Slider_Fill.color = blue_Color;
            UpArrow_Exp.SetActive(false);
            return false; // MAX는 "레벨업 가능" 개념이 없다고 보면 false
        }

        int expForNextLevel = HeroManager.Instance.MaxExpSetting(level);

        // ✅ 안전장치 (혹시 0 반환하면 Slider가 이상해질 수 있음)
        if (expForNextLevel <= 0) expForNextLevel = 1;

        exp_slider.maxValue = expForNextLevel;
        exp_slider.value = Mathf.Clamp(exp, 0, expForNextLevel);
        slider_text.text = $"{(int)exp_slider.value}/{(int)exp_slider.maxValue}";

        bool readyToLevelUp = exp >= expForNextLevel; // ✅ == 말고 >=

        exp_Slider_Fill.color = readyToLevelUp ? green_Color : blue_Color;
        UpArrow_Exp.SetActive(readyToLevelUp);

        return readyToLevelUp;
    }

    private bool UpdateUpgradeExpSlider()
    {
        int expForNextLevel = HeroManager.Instance.MaxUpgradeExpSetting(rank);
        upgrade_slider.maxValue = expForNextLevel;
        upgrade_slider.value = Mathf.Min(UpgradeStone, expForNextLevel);
        upgrade_slider_text.text = $"{UpgradeStone}/{upgrade_slider.maxValue}";
        if(UpgradeStone == expForNextLevel)
        {
            upgrade_Slider_Fill.color = green_Color;
            UpArrow_Upgrade.SetActive(true);
            return true;//레벨업 준비 완
        }
        else
        {
            upgrade_Slider_Fill.color = blue_Color;
            UpArrow_Upgrade.SetActive(false);
            return false;//레벨업 준비 미완
        }
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

    public void Init_LevelInfo()
    {
        HeadView.SetActive(true);
        MiddleView_Level.SetActive(true);
        MiddleView_Rank.SetActive(false);
        lines[0].SetActive(true);
        lines[1].SetActive(true);
        lines[2].SetActive(false);
        BottomView_Level.SetActive(true);
        BottomView_Rank.SetActive(false);
        exp_slider.gameObject.SetActive(true);
        upgrade_slider.gameObject.SetActive(false);
        upgradeStoneLabel.gameObject.SetActive(false);
        rankUpButton.gameObject.SetActive(false);
        heroLevelUpButton.gameObject.SetActive(true);
        var p = MileStoneScroll.anchoredPosition;//스크롤 맨 위로
        p.y = 0f;
        MileStoneScroll.anchoredPosition = p;
        LevelInfoButton.color = DarkPupple_Color;
        RankInfoButton.color = Pupple_Color;
    }
    public void Init_RankInfo()
    {
        HeadView.SetActive(true);
        MiddleView_Level.SetActive(false);
        MiddleView_Rank.SetActive(true);
        lines[0].SetActive(true);
        lines[1].SetActive(false);
        lines[2].SetActive(true);
        BottomView_Level.SetActive(false);
        BottomView_Rank.SetActive(true);
        exp_slider.gameObject.SetActive(false);
        upgrade_slider.gameObject.SetActive(true);
        upgradeStoneLabel.gameObject.SetActive(true);
        rankUpButton.gameObject.SetActive(true);
        heroLevelUpButton.gameObject.SetActive(false);
        int RequireUpgradeStone = HeroManager.Instance.MaxUpgradeExpSetting(rank);
        rankUpButton.Init(RequireUpgradeStone);

        // var p = MileStoneScroll.anchoredPosition;//스크롤 맨 위로
        // p.y = 0f;
        // MileStoneScroll.anchoredPosition = p;
        LevelInfoButton.color = Pupple_Color;
        RankInfoButton.color = DarkPupple_Color;
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
            //Debug.Log($"Base Font = {t.font?.name}");
            // if (t.font != null && t.font.fallbackFontAssetTable != null)
            // {
            //     //Debug.Log("Fallbacks:");
            //     foreach (var f in t.font.fallbackFontAssetTable)
            //         Debug.Log($" - {f?.name}");
            // }

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