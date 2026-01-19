using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;

public class HeroLevelUpButton : MonoBehaviour
{
    [Header("REFS")]
    public TMP_Text main;            // 메인 텍스트(EXP+1 or 레벨업)
    public TMP_Text value;           // 필요 재화량
    public Image lockBG;             // 잠금 배경(어두운)
    public Image valueBG;            // 재화량 배경
    public TMP_Text MaxLevelText;    // 최대레벨 텍스트

    [Header("Localization (LocalizeStringEvent)")]
    [SerializeField] private LocalizeStringEvent mainLocalize;

    [Header("String Table")]
    [SerializeField] private string stringTableName = "UI_Common"; // ✅ 너 프로젝트 테이블명으로 수정

    [Header("Keys")]
    [SerializeField] private string tableKey_LEVUPUp = "Skill.LevelUp";
    [SerializeField] private string tableKey_EXPUp = "EXPup";

    [Header("Data")]
    public ServerDataManager.HeroAccount heroData;
    private int requireGold;
    private int nowExp;
    private int maxExp;
    private bool nowLevelUp;
    private int nowLevel;

    private const int MAX_LEVEL = 20;

    [Header("Connect")]
    public HeroInfo heroInfo;
    public SkillPanel skillPanel;

    public void Init(ServerDataManager.HeroAccount data)
    {
        heroData = data;

        nowLevel = heroData.level;
        nowExp   = heroData.exp;

        maxExp = HeroManager.Instance.MaxExpSetting(nowLevel);
        nowLevelUp = (nowExp >= maxExp);

        requireGold = HeroManager.Instance.GetRequirementsGold(nowLevelUp);

        // ✅ 초기 UI 반영
        MainTextSetting();
        ValueTextSetting();
    }

    public void MainTextSetting()
    {
        bool isMaxLevel = (nowLevel >= MAX_LEVEL);

        // ✅ MaxLevel이면 main 숨기고 MaxLevelText 표시
        if (main != null) main.gameObject.SetActive(!isMaxLevel);
        if (MaxLevelText != null) MaxLevelText.gameObject.SetActive(isMaxLevel);

        // ✅ max level이 아니면 main 로컬라이즈
        string key = (nowExp < maxExp) ? tableKey_EXPUp : tableKey_LEVUPUp; // nowExp == maxExp 포함

        if (mainLocalize != null)
        {
            mainLocalize.StringReference = new LocalizedString(stringTableName, key);
            mainLocalize.RefreshString();
        }
        else
        {
            // LocalizeStringEvent가 없으면 fallback (임시)
            main.text = key;
        }
    }

    public void ValueTextSetting()
    {
        int currentGold = ServerDataManager.instance.GetCurrentGold();
        bool canPay = currentGold >= requireGold;

        lockBG.gameObject.SetActive(!canPay);
        value.color = canPay ? Color.white : Color.red;
        value.text = requireGold.ToString();

        // ✅ 최대레벨이면 재화 BG 숨기고 MaxLevelText 표시 (요청 조건)
        bool isMaxLevel = (nowLevel >= MAX_LEVEL);
        valueBG.gameObject.SetActive(!isMaxLevel);
        MaxLevelText.gameObject.SetActive(isMaxLevel);
        lockBG.gameObject.SetActive(isMaxLevel);

        // (선택) max level 상태일 때 메인도 같이 세팅 맞춰주기
        if (isMaxLevel && main != null)
            main.gameObject.SetActive(false);
    }

    public void ExpUpButton()
    {
        int id = heroData.heroId;
        var heroSO = HeroManager.Instance.GetHeroSO(id);
        if (heroSO == null)
        {
            Debug.LogError($"[HeroInfo] Hero SO not found for id={id}");
            return;
        }

        if(heroData.level >= MAX_LEVEL)
        {
            Debug.LogWarning("[HeroInfo] Cannot purchase exp - already at max level!");
            return;
        }

        int currentGold = ServerDataManager.instance.GetCurrentGold();
        bool canPay = currentGold >= requireGold;
        ServerDataManager.instance.BuyExp(id, maxExp,requireGold);

        int nowExp = ServerDataManager.instance.GetCurrentExp(id);
        int max = HeroManager.Instance.MaxExpSetting(heroData.level);

        if(nowExp > max)
            ServerDataManager.instance.LevelUp(id);

        if (canPay)
        {
            // ✅ root에서 값 다시 읽고 UI 갱신
            heroInfo.RefreshUI();
            skillPanel.RefreshUI();
            Debug.Log($"[HeroInfo] Exp purchased! id={id}");
        }
        else
        {
            Debug.LogWarning("[HeroInfo] Failed to purchase exp - not enough gold!");
        }
    }
}
