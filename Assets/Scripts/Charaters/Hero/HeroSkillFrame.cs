using Game.RankSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SkillSlotType { C, R, E, L, M }

public class HeroSkillFrame : MonoBehaviour
{
    [Header("REF")]
    public Image frame;
    public Image icon;
    public TMP_Text text_Lv;
    public Slider expSlider;
    public TMP_Text text_exp;
    public Image upArrow;
    public Image lockImage;//좌물쇠

    [Header("Type")]
    public SkillSlotType slotType;   // ✅ 인스펙터에서 C/R/E/L/M 지정

    [Header("Data")]
    public ServerDataManager.HeroAccount data;
    public HeroScriptableObject heroScriptableObject;

    public void Init(ServerDataManager.HeroAccount heroAccount)
    {
        data = heroAccount;
        heroScriptableObject = GameManager.instance.heroManager.heroes[data.heroId];
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (data == null) return;

        int lv = GetLevelByType(data, slotType);
        int exp = GetExpByType(data, slotType);

        text_Lv.text = $"Lv.{lv}";
        // expSlider는 “다음 레벨 필요 exp”가 있어야 정확히 표현 가능
        // 일단 임시로 0~10 기준 예시:
        int maxExp = 10; // TODO: 너 규칙으로 바꾸기
        expSlider.maxValue = maxExp;
        expSlider.value = Mathf.Clamp(exp, 0, maxExp);
        text_exp.text = exp.ToString() + " / " + maxExp;

        // 레벨업 가능 표시(예시)
        bool canLevelUp = exp >= maxExp;
        if (upArrow != null) upArrow.gameObject.SetActive(canLevelUp);

        //아이콘 세팅
        icon.sprite = GetSkillIconBySlotType();

        //프레임 세팅
        frame.color = RankDatas.GetColor(GetRankBySlotType());

         // ✅ 잠금 처리: 현재 히어로 등급보다 높은 슬롯이면 잠금
        UpdateLockState();
    }
        private Sprite GetSkillIconBySlotType()
    {
        if (heroScriptableObject == null || heroScriptableObject.rankUnlocks == null)
            return null;

        RankType targetRank = slotType switch
        {
            SkillSlotType.C => RankType.Uncommon,
            SkillSlotType.R => RankType.Rare,
            SkillSlotType.E => RankType.Epic,
            SkillSlotType.L => RankType.Legendary,
            SkillSlotType.M => RankType.Mythic,
            _ => RankType.Uncommon
        };

        // ✅ 리스트에서 rank가 일치하는 항목 찾기 (순서 상관 없음)
        var unlock = heroScriptableObject.rankUnlocks.Find(u => u.rank == targetRank);
        return unlock != null ? unlock.skillSpite : null;
    }

    private int GetLevelByType(ServerDataManager.HeroAccount h, SkillSlotType t) => t switch
    {
        SkillSlotType.C => h.cSkillLevel,
        SkillSlotType.R => h.rSkillLevel,
        SkillSlotType.E => h.eSkillLevel,
        SkillSlotType.L => h.lSkillLevel,
        SkillSlotType.M => h.mSkillLevel,
        _ => 1
    };
        private RankType GetRankBySlotType() => slotType switch
    {
        SkillSlotType.C => RankType.Uncommon,
        SkillSlotType.R => RankType.Rare,
        SkillSlotType.E => RankType.Epic,
        SkillSlotType.L => RankType.Legendary,
        SkillSlotType.M => RankType.Mythic,
        _ => RankType.Uncommon
    };

    private int GetExpByType(ServerDataManager.HeroAccount h, SkillSlotType t) => t switch
    {
        SkillSlotType.C => h.cSkillExp,
        SkillSlotType.R => h.rSkillExp,
        SkillSlotType.E => h.eSkillExp,
        SkillSlotType.L => h.lSkillExp,
        SkillSlotType.M => h.mSkillExp,
        _ => 0
    };

        private void UpdateLockState()
    {
        if (lockImage == null) return;

        RankType slotRank = GetRankBySlotType();           // 이 슬롯이 요구하는 등급
        RankType heroOpenRank = (RankType)data.rank;       // 현재 오픈된 히어로 등급 (int라면 캐스팅)

        bool isLocked = heroOpenRank < slotRank;           // 슬롯이 더 높으면 잠금
        lockImage.gameObject.SetActive(isLocked);

        // (선택) 잠겨있으면 레벨업 화살표도 숨기고 싶으면:
        if (upArrow != null && isLocked)
            upArrow.gameObject.SetActive(false);
    }
}
